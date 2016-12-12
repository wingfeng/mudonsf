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
    public class Player
    {
        ContainerBuilder builder = new ContainerBuilder();
        IContainer container;
        public PlayerState State { get; set; }
        public TelnetListener Server { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        private Thread nofityThread;
        private Queue<string> msgToDelivery;
        private NetworkStream clientStream;
        private DateTime lastActive;
        private Uri roomServiceUri=new Uri("fabric:/MudOnSF/RoomActorService");

        internal TcpClient client { get; set; }
        public PlayerStatus Status { get; set; }
        public Player(TcpClient client) {
            State = new PlayerState()
            {
                Name = "name of player",
                Description = "Description of player"
            };

            this.client = client;
            nofityThread = new Thread(new ThreadStart(notifyToClient));
            msgToDelivery = new Queue<string>();
            nofityThread.Start();
            builder.RegisterType<SayCommand>().AsImplementedInterfaces().Named<ICommand>("say");
            builder.RegisterType<ShoutCommand>().AsImplementedInterfaces().Named<ICommand>("fy");
            builder.RegisterType<LookCommand>().AsImplementedInterfaces().Named<ICommand>("look");
            container = builder.Build();
        }
        public StringBuilder Messages { get; set; }
   
        public void Notify(string msg,bool withPrompt=true)
        {
            msgToDelivery.Enqueue(msg);
            if (withPrompt)
            {
               
                msgToDelivery.Enqueue(string.Format("{0}:{1}>",client.Client.LocalEndPoint, LoginId));
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
            string msg = "请输入你的账号:";
            Notify(msg);
            var strAccount = client.ReadCommand();
            msg = "请输入你的密码：";
            Notify(msg);
            var strPassword = client.ReadCommand();
            switch (checkAccount(strAccount, strPassword))
            {
                case PlayerStatus.NotExist:
                    //create user;
                    break;
                case PlayerStatus.login:
                    // Init First Room;
                    this.State.Name = strAccount;
                    IRoomActor roomActor = ActorProxy.Create<IRoomActor>(new Microsoft.ServiceFabric.Actors.ActorId("root"), roomServiceUri);
                    var room = await roomActor.Enter(this.State, "");
                    State.RoomId = room.Id;
                    //msg = "欢迎来到第一个Room\r\n";
                    Notify(room.Name +"\r\n",false);
                    Notify(room.Description);
                    waitForCommand();
                    break;
            }
            

        }

        private PlayerStatus checkAccount(string strAccount, string strPassword)
        {
            return PlayerStatus.login;
        }

        private void waitForCommand()
        {
            while(this.Status== PlayerStatus.login)
            {
                string command = client.ReadCommand();
                parseCommand(command);
            }
        }
        private void parseCommand(string strCommand)
        {
           
            if (string.IsNullOrWhiteSpace(strCommand))
                return;
            string expression = @"(?<command>\w+)\s?(?<body>.*$)?";
            Regex reg = new Regex(expression);
            var match = reg.Match(strCommand);
            var commandName = match.Groups["command"].Value;
            var body = match.Groups["body"].Value;
            Type commandType = this.GetType().Assembly.GetType(string.Format("GameServer.{0}Command", commandName));

            ICommand command = null;
            try
            {
                command = container.ResolveNamed<ICommand>(commandName, new NamedParameter("_invoker", this), new NamedParameter("body", body));
            }
            catch(Autofac.Core.Registration.ComponentNotRegisteredException err)
            {
                //do nothing;
            }
            if(command==null)
            {
                Notify("我不懂你在说什么!\r\n");
                return;
            }
            command.Execute();
        }
        internal void Initialize(object obj)
        {
            Notify("Welcome to the world!\r\n请输入回车键继续你的冒险之旅!\r\n",false);
            client.ReadCommand();
            Login();
            //Do something here;ss
        }
    }
}
