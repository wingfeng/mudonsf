using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityService.Interface.State
{
   public class RoomState
    {
        public RoomState()
        {
            Players = new List<string>();
        }
       public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Players { get; set; }
    }
}
