using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityService.Interface
{
   public interface IWorld
    {
        IRoom GetRoom(string roomID);
       
    }
}
