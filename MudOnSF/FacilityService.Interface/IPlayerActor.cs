using FacilityService.Interface.State;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityService.Interface
{
    public interface IPlayerActor: IActor
    {
        Task SetState(PlayerState state);
        Task<PlayerState> GetState();

        Task<bool> CheckPassword(string password);
        Task Kill(ICharacter target);
        Task Look(ICharacter target);
        Task Notify(string msg,bool withPrompt);
    }
}
