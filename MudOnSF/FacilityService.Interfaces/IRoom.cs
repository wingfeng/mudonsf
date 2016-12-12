using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityService.Interfaces
{
    public interface IRoom
    {
        /// <summary>
        /// 获取当前房间的玩家
        /// </summary>
        /// <returns></returns>
        string GetCharacters();
        /// <summary>
        /// 获取房间内的物品
        /// </summary>
        /// <returns></returns>
        string GetItems();       
    }
}
