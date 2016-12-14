using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class Command : ICommand
    {
        public PlayerProxy Invoker { get; set; }
        object body;
        public Command(object _invoker,object body)
        {
            Invoker = _invoker as PlayerProxy;
        }
   
        public virtual void Execute() { }
    }
}
