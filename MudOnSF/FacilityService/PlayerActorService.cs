using FacilityService.Interface;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacilityService.Interface.State;
using Microsoft.ServiceFabric.Actors;
using GameServer.Interface;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;

namespace FacilityService
{
    public class PlayerActorService : Actor, IPlayerActor
    {
        public PlayerActorService(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
            if (actorId.GetStringId() == "wing")
            {
                State = new PlayerState()
                {
                    Name = "wing",
                    Description = "天神",
                    PasswordHash = "Pass@word0"

                };

            }
            else
            {
                State = new PlayerState()
                {
                    Name = actorId.GetStringId(),
                    Description = "普通百姓",
                    PasswordHash = "12345"

                };
            }

        }


        public PlayerState State
        {
            get; set;

        }

        public async Task SetState(PlayerState state)
        {
            State = state;
        }

        public   async Task<bool> CheckPassword(string password)
        {

#if (DEBUG)
            return true;
#endif
            return string.Equals(State.PasswordHash, password, StringComparison.CurrentCulture);
        }

        public async Task<PlayerState> GetState()
        {
            return State;
        }

        Task IPlayerActor.Kill(ICharacter target)
        {
            throw new NotImplementedException();
        }

        Task IPlayerActor.Look(ICharacter target)
        {
            throw new NotImplementedException();
        }

        public async Task Notify(string msg, bool withPrompt)
        {
            string uri = string.Format("fabric:/MudOnSF/GameServer", State.Server);
            ServicePartitionResolver resolver = ServicePartitionResolver.GetDefault();
          
                IGameServer gameServer = ServiceProxy.Create<IGameServer>(new Uri(uri),new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(11111l),listenerName:"RPCEndpoint");
            await gameServer.NotifyPlayer(State.Name, msg, withPrompt);
        }
    }
}
