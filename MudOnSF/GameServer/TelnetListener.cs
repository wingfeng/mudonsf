using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Fabric;
using FacilityService.Interface;

namespace GameServer
{
   public class TelnetListener : ICommunicationListener
    {
        StatefulServiceContext _context;
        internal GameServer Server { get; set; }
        private Thread listenThread;
        private TcpListener tcpListener;
        private List<Thread> playerThreadList = new List<Thread>();
        private List<PlayerProxy> sessions = new List<PlayerProxy>();
        private Dictionary<string, PlayerProxy> players = new Dictionary<string, PlayerProxy>();
        public Dictionary<string, PlayerProxy> Players
        {
            get { return players; }
        }

        public List<PlayerProxy> Sessions
        {
            get { return sessions; }
        }
        private object playerLocker = new object();
        public PlayerProxy GetProxy(string name)
        {
            foreach(var p in Sessions)
            {
                if (string.Equals(p.State.Name, name, StringComparison.CurrentCultureIgnoreCase))
                    return p;
            }
            return null;
        }
        public TelnetListener(StatefulServiceContext context) {
            _context = context;
            System.Diagnostics.Debugger.Launch();
        }
        
        public void Abort()
        {
            //do nothing;
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            //do nothing;
            return Task.FromResult(0);
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            //Todo:Build a Client Thread Pool;
            this.tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), 9980);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
            return Task.FromResult ("");
        }

        private void ListenForClients()
        {
            //start our listener socket
            this.tcpListener.Start();

            //start infinite listening loop
            while (true)
            {
                //loop blocks, waiting to accept client
                TcpClient client = this.tcpListener.AcceptTcpClient();

                //when it does, we'll create a new player instance, passing in some stuff
                PlayerProxy player = new PlayerProxy(client) { Server = this.Server
                };
                
                player.OnPlayerLogined += Player_OnPlayerLogined;
                //then let's create a new thread and initialize our player instance
                Thread clientThread = new Thread(new ParameterizedThreadStart(player.Initialize));
                clientThread.Start();

                //add the thread to the thread list so we can track it if necessary
                lock (playerThreadList)
                {
                    this.playerThreadList.Add(clientThread);
                }

                //let's assign some event handlers for the player connecting and disconnecting
            

                //and add the player to the player list.
                lock (playerLocker)
                {
                    this.sessions.Add(player);
                }
            }
        }

        private void Player_OnPlayerLogined(object sender, EventArgs e)
        {
            var playerProxy = sender as PlayerProxy;
            if(!players.ContainsKey(playerProxy.State.Name))
             this.players.Add(playerProxy.State.Name, playerProxy);
        }

        public void RemovePlayer(PlayerProxy player)
        {
            lock (playerLocker)
            {
                this.sessions.Remove(player);
            }
        }
      
    }
}
