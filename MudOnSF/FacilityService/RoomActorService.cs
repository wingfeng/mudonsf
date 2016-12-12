using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using FacilityService.Interface;
using FacilityService.Interface.State;

namespace FacilityService
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class RoomActorService : Actor, IRoomActor
    {
        public RoomState State{get;set;}
        /// <summary>
        /// Initializes a new instance of FacilityService
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public RoomActorService(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {

       //     State.Id = actorId.ToString();
        }

        public async Task<RoomState> Enter(PlayerState player,string name="")
        {
             State = await this.StateManager.GetStateAsync<RoomState>("State") as RoomState;
           
            if (State == null && string.IsNullOrWhiteSpace(name))
            {
                var tmpState = new RoomState()
                {
                    Name = "有间客栈",
                    Description = "前厅挂着一幅龙凤双飞的巨画。当门挂着对鸳鸯球，球上系着几个小小的黄铜风铃。微风掠过，风铃发出清脆悦耳的叮咚声。一个满面春风的小二正忙碌地\x1B[37m招待\x1B[32m"
                                   + "顾客，案桌上贴着张\x1B[37m白纸\x1B[32m，上面龙飞凤舞地写着几个草字。走出大门，迎面便是斗"
                               + "大的大红\x1B[37m灯笼[32m，灯笼下一口\x1B[37m大缸\x1B[32m，想是为往来行人提供清水。\r\n",
                    Id = this.Id.ToString()
                    
                    
                };
                State = tmpState;
            }
            State.Players.Add(player.Name);
            StateManager.SetStateAsync<RoomState>("State", State);

            return State;
        }
        public Task Leave(PlayerState player)
        {
            State.Players.Remove(player.Name);
            return Task.FromResult(0);
        }
        public async Task<RoomState> GetRoom(string roomId)
        {
            
           var tmp=await this.StateManager.GetStateAsync < RoomState > ("State");
            var state = tmp as RoomState;
            return state;
        }

        public Task<List<KeyValuePair<string, string>>> QueryEntry()
        {
            throw new NotImplementedException();
        }

        public Task<PlayerState> QueryPlayer(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync<RoomState>("State", State);
        }
    }
}
