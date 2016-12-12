using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityService.Interfaces
{
    public interface IPlayer
    {
        void Kill(ICharacter target);
        void Look(ICharacter target);
    }
}
