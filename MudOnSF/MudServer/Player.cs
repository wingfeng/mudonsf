using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MudServer
{
    public class Player
    {
        private NetworkStream clientStream;
        private DateTime lastActive;
        public TcpClient client;
        public Player(TcpClient client) { }
        public StringBuilder Messages { get; set; }
        public void NotifyToClient()
        {

        }
    }
}
