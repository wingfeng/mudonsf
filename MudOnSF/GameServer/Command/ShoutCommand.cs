using GameServer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class ShoutCommand : Command
    {
        string msg;
        public ShoutCommand(object _invoker,object body) : base(_invoker,body)
        {
            msg = string.Format("{0}", body);
        }

        public override void Execute()
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                Invoker.Notify("你想说什么？\r\n");
                return;
            }
           
            var i = msg.LastIndexOf("\r\n");
            if (i < 0)
                msg = string.Format("\x1B[0;1;33m风云:{0}\x1B[0m\r\n", msg);

            foreach (KeyValuePair<string,PlayerProxy> p in Invoker.Server.Players)
            {
                p.Value.Notify(msg,false,true);
            }
        }
    }
}
