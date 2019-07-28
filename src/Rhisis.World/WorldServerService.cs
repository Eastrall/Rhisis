using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Rhisis.World
{
    public sealed class WorldServerService : IHostedService
    {
        private readonly IWorldServer _worldServer;

        /// <summary>
        /// Creates a new <see cref="WorldServerService"/> instance.
        /// </summary>
        /// <param name="worldServer">World Server.</param>
        public WorldServerService(IWorldServer worldServer)
        {
            this._worldServer = worldServer;
        }

        /// <summary>
        /// Starts the world server service.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._worldServer.Start();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the world server service.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._worldServer.Stop();

            return Task.CompletedTask;
        }
    }
}