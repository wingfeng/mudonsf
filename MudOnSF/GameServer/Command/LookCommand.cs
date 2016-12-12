using FacilityService.Interface;
using FacilityService.Interface.State;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class LookCommand:Command
    {
        private Uri roomServiceUri = new Uri("fabric:/MudOnSF/RoomActorService");
        Player Invoker;
        public LookCommand(object _invoker,object body) : base(_invoker, body) {
            Invoker = _invoker as Player;
        }
        public async override void Execute()
        {
            IRoomActor roomActor = ActorProxy.Create<IRoomActor>(new Microsoft.ServiceFabric.Actors.ActorId(Invoker.State.RoomId), roomServiceUri);
           var room = await roomActor.GetRoom(Invoker.State.RoomId);
            var players = room.Players;
            Invoker.Notify(room.Name + "\r\n", false);
            Invoker.Notify(room.Description,false);
            string msg = "房间里有\r\n----------\r\n";
            foreach(string p in players)
            {
                msg += p + "\r\n";
                
            }
            msg += "----------\r\n";
            Invoker.Notify(msg);
        }
    }
}
