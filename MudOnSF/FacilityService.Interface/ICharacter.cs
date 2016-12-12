using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityService.Interface
{
    public interface ICharacter
    {
        int HP { get; set; }
        int MP { get; set; }
        void Die();
        void Say();
        
    }
}
