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

namespace GameServer
{
   public class TelnetListener : ICommunicationListener
    {
        private Thread listenThread;
        private TcpListener tcpListener;
        private List<Thread> playerThreadList = new List<Thread>();
        private List<Player> players = new List<Player>();
        public List<Player> Players
        {
            get { return players; }
        }
        private object playerLocker = new object();

        public TelnetListener(StatefulServiceContext context) { }
        //ThreadPool ClientThreads = new ThreadPool();
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
            this.tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), 3000);
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
                Player player = new Player(client) { Server = this };

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
                    this.players.Add(player);
                }
            }
        }

        public void Shout(Player player,string Message)
        {
        
        }
    }
}
