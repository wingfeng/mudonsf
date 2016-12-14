using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Interface
{
    public interface IGameServer:IService
    {
        Task NotifyPlayer(string PlayerName, string msg,bool withPrompt);
    }
}
