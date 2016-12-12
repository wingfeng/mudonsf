﻿using System;
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
                return;

            Player owner = Invoker as Player;
            var i = msg.LastIndexOf("\r\n");
            if (i < 0)
                msg = string.Format("\x1B[0;1;33m风云:{0}\x1B[0m\r\n", msg);
            foreach (Player p in owner.Server.Players)
            {
                p.Notify(msg);
            }
        }
    }
}