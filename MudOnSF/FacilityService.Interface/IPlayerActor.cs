using FacilityService.Interface.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityService.Interface
{
    public interface IPlayerActor
    {
        PlayerState GetState();
        string CurrentRoom { get; set; }
        void Kill(ICharacter target);
        void Look(ICharacter target);
    }
}
