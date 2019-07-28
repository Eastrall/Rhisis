using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Rhisis.Cluster
{
    public sealed class ClusterServerService : IHostedService
    {
        private readonly ILogger<ClusterServerService> _logger;
        private readonly IClusterServer _clusterServer;

        /// <summary>
        /// Creates a new <see cref="ClusterServerService"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="clusterServer">Cluster Server instance.</param>
        public ClusterServerService(ILogger<ClusterServerService> logger, IClusterServer clusterServer)
        {
            this._logger = logger;
            this._clusterServer = clusterServer;
        }

        /// <summary>
        /// Starts the cluster server service.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"Starting {nameof(ClusterServer)}.");
            this._clusterServer.Start();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the cluster server service.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"Stopping {nameof(ClusterServer)}.");
            this._clusterServer.Stop();

            return Task.CompletedTask;
        }
    }
}