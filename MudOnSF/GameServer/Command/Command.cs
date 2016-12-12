using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class Command : ICommand
    {
        object invoker;
        object body;
        public Command(object _invoker,object body)
        {
            invoker = _invoker;
        }
        public Object Invoker { get { return invoker; } }
        public virtual void Execute() { }
    }
}
