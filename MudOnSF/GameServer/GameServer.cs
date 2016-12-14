using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using GameServer.Interface;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
namespace GameServer
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class GameServer : StatefulService, IGameServer
    {
        private TelnetListener GameListener;
        public GameServer(StatefulServiceContext context)
            : base(context)
        { }

        public async Task NotifyPlayer(string playerName, string msg,bool withPrompt)
        {
            var pProxy = GameListener.GetProxy(playerName);
            if (pProxy != null)
            {
                if(pProxy.State.Server==Context.NodeContext.IPAddressOrFQDN)
                     pProxy.Notify(msg, true,withPrompt);
            }
        }
        public Dictionary<string, PlayerProxy> Players
        {
            get { return GameListener.Players; }
        }
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            var rpcListener = this.CreateServiceRemotingListener<GameServer>(Context);
           
            GameListener = new TelnetListener(Context) { Server = this };

            var rpcEndpoint=new ServiceReplicaListener(context=>rpcListener,"RPCEndpoint");
            
            var telnetListener = new ServiceReplicaListener(context =>
                      GameListener,
                 "TelnetEndpoint");



            return new ServiceReplicaListener[] { rpcEndpoint, telnetListener };
        }


        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            //// TODO: Replace the following sample code with your own logic 
            ////       or remove this RunAsync override if it's not needed in your service.

            //var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            //while (true)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    using (var tx = this.StateManager.CreateTransaction())
            //    {
            //        var result = await myDictionary.TryGetValueAsync(tx, "Counter");

            //        ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
            //            result.HasValue ? result.Value.ToString() : "Value does not exist.");

            //        await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

            //        // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
            //        // discarded, and nothing is saved to the secondary replicas.
            //        await tx.CommitAsync();
            //    }

            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //}
        }


    }
}
