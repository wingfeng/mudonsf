using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameServer;
using FacilityService.Interface.Enum;
using System.Threading;
using System.Text.RegularExpressions;
using Autofac;
using Autofac.Core;
using FacilityService.Interface;
using Microsoft.ServiceFabric.Actors.Client;
using FacilityService.Interface.State;

namespace GameServer
{
    public class PlayerProxy
    {
        ContainerBuilder builder = new ContainerBuilder();
        IContainer container;
        public PlayerState State { get; set; }
        internal GameServer Server { get; set; }
        IPlayerActor actor;
        public string Password { get; set; }
        private Thread nofityThread;
        private Thread commandThread;
        private Queue<string> msgToDelivery;
        public event EventHandler OnPlayerLogined;

        private Uri roomServiceUri = new Uri("fabric:/MudOnSF/RoomActorService");
        private Uri playerServiceUri = new Uri("fabric:/MudOnSF/PlayerActorService");
        internal TcpClient client { get; set; }

        public PlayerProxy(TcpClient client)
        {

            State = new PlayerState()
            {
                Name = "",
                Description = "Description of player"
            };

            this.client = client;
            nofityThread = new Thread(new ThreadStart(notifyToClient));
            msgToDelivery = new Queue<string>();
            nofityThread.Start();
            builder.RegisterType<SayCommand>().AsImplementedInterfaces().Named<ICommand>("say");
            builder.RegisterType<ShoutCommand>().AsImplementedInterfaces().Named<ICommand>("fy");
            builder.RegisterType<LookCommand>().AsImplementedInterfaces().Named<ICommand>("look");
            builder.RegisterType<QuitCommand>().AsImplementedInterfaces().Named<ICommand>("quit");
            container = builder.Build();
        }


        public void Notify(string msg, bool isLocal = false, bool withPrompt = true)
        {
            if (isLocal)
            {
                DeliverMessage(msg, withPrompt);
            }
            else
                actor.Notify(msg, withPrompt);
        }
        private async void DeliverMessage(string msg, bool withPrompt = true)
        {
            msgToDelivery.Enqueue(msg);
            if (withPrompt)
            {

                msgToDelivery.Enqueue(string.Format("{0}:{1}>", client.Client.LocalEndPoint, State.Name));
            }
        }
        private void notifyToClient()
        {
            while (client.Connected)
            {
                if (msgToDelivery.Count > 0)
                {
                    string msg = msgToDelivery.Dequeue();
                    if (!string.IsNullOrWhiteSpace(msg))
                        client.WriteMessage(msg);

                }
                Thread.Sleep(10);
            }
        }
        public async void Login()
        {
            bool accountValidated = false;
            string strAccount = "";
            string msg = "请输入你的账号:";
            while (!accountValidated)
            {

                Notify(msg, true, false);
                strAccount = client.ReadCommand();
                var match = Regex.Match(strAccount, "^[a-zA-z][a-zA-Z0-9_]{3,9}$");
                if (!string.IsNullOrWhiteSpace(match.Value))
                {
                    strAccount = match.Value;
                    break;
                }
                msg = "账号的格式非法，应该是以字母开头以下划线或者数组组合不少于4个字符不超过9个字符的字串！\r\n";
                Notify(msg, true);
            }
            msg = "请输入你的密码：";
            Notify(msg, true,false);
            actor = ActorProxy.Create<IPlayerActor>(new Microsoft.ServiceFabric.Actors.ActorId(strAccount), playerServiceUri);

            var strPassword = client.ReadCommand();
            if (await actor.CheckPassword(strPassword))
            {


                // Init First Room;
                this.State.Name = strAccount;
                IRoomActor roomActor = ActorProxy.Create<IRoomActor>(new Microsoft.ServiceFabric.Actors.ActorId("root"), roomServiceUri);
                var room = await roomActor.Enter(this.State, "");
                State.RoomId = room.Id;
                //msg = "欢迎来到第一个Room\r\n";
                Notify(room.Name + "\r\n", true, false);
                Notify(room.Description, true);
                State.Server = Server.Context.NodeContext.IPAddressOrFQDN;
                State.Status = PlayerStatus.Active;
                //触发用户登入事件；
                OnPlayerLogined?.Invoke(this, new EventArgs() { });

                await actor.SetState(State);
                commandThread = new Thread(new ThreadStart(waitForCommand));
                commandThread.Start();

            }
            else
            {
                Notify("账号密码错误，再见!", true, false);
                Quit();
            }

        }

        private PlayerStatus checkAccount(string strAccount, string strPassword)
        {
            return PlayerStatus.Active;
        }

        private void waitForCommand()
        {
            while (this.State.Status == PlayerStatus.Active)
            {
                string cmd = client.ReadCommand();
                parseCommand(cmd);
                this.State.LastActive = DateTime.Now;
                waitForCommand();
            }
        }
        private void parseCommand(string strCommand)
        {

            //if (string.IsNullOrWhiteSpace(strCommand))
            //    return;
            string expression = @"(?<command>\w+)\s?(?<body>.*$)?";
            Regex reg = new Regex(expression);
            var match = reg.Match(strCommand);
            var commandName = match.Groups["command"].Value;
            if (!string.IsNullOrWhiteSpace(commandName))
                commandName = commandName.ToLower();
            else
            {
                Notify("我不懂你在说什么!\r\n", true);
                return;
            }
            var body = match.Groups["body"].Value;
            Type commandType = this.GetType().Assembly.GetType(string.Format("GameServer.{0}Command", commandName));

            ICommand command = null;
            try
            {
                command = container.ResolveNamed<ICommand>(commandName, new NamedParameter("_invoker", this), new NamedParameter("body", body));
            }
            catch (Autofac.Core.Registration.ComponentNotRegisteredException err)
            {
                //do nothing;
            }
            if (command == null)
            {
                Notify("我不懂你在说什么!\r\n", true);
                return;
            }
            command.Execute();
        }
        internal void Initialize(object obj)
        {
            Notify("Welcome to the world!\r\n请输入回车键继续你的冒险之旅!\r\n", true, false);
            client.ReadCommand();
            Login();
            //Do something here;ss
        }

        public void Quit()
        {
            this.State.Status = PlayerStatus.Quit;

            Thread.Sleep(10);
            //  this.Server.RemovePlayer(this);
            try
            {
                if (commandThread != null && commandThread.IsAlive)
                    this.commandThread.Abort();
                if (nofityThread != null && nofityThread.IsAlive)
                    this.nofityThread.Abort();
            }
            finally
            {
                this.client.Close();
            }
        }
    }
}
