using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Rhisis.World.CoreClient
{
    public sealed class WorldCoreClientService : IHostedService
    {
        private readonly IWorldCoreClient _worldCoreClient;

        /// <summary>
        /// Creates a new <see cref="WorldCoreClientService"/> instance.
        /// </summary>
        /// <param name="worldCoreClient">World core client.</param>
        public WorldCoreClientService(IWorldCoreClient worldCoreClient)
        {
            this._worldCoreClient = worldCoreClient;
        }

        /// <summary>
        /// Starts the world core client service.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._worldCoreClient.Connect();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the world core client service.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._worldCoreClient.Disconnect();

            return Task.CompletedTask;
        }
    }
}
