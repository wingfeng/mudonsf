using FacilityService.Interface.State;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityService.Interface
{
    public interface IRoomActor:IActor
    {
        Task<RoomState> GetRoom(string roomId);
        Task<RoomState> Enter(PlayerState player,string name);
        Task Leave(PlayerState player);
        /// <summary>
        /// 查询房间内的玩家
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<PlayerState> QueryPlayer(string name);
        /// <summary>
        /// 查询出口
        /// </summary>
        /// <returns></returns>
       Task< List<KeyValuePair<string,string>>> QueryEntry();
    }
}
