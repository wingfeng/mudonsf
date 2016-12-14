using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    class QuitCommand : Command
    {
        public QuitCommand(object _invoker, object body) : base(_invoker, body)
        {
            
        }
        public override void Execute()
        {
            Invoker.Notify("你如一缕青烟一样散去了\r\n");           
            Invoker.Quit();
        }
    }
}
