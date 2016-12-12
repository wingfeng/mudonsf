using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MudServer
{
    class TelnetListener : ICommunicationListener
    {
        public void Abort()
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
