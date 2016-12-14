using FacilityService.Interface.Enum;
using Microsoft.ServiceFabric.Services.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityService.Interface.State
{
    public class PlayerState
    {
        public string Server { get; set; }
        public string RoomId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PlayerStatus Status { get; set; }
        public string PasswordHash { get; set; }
        public DateTime LastActive { get; set; }

        public ServicePartitionKey PartitionKey { get; set; }
    }
}
