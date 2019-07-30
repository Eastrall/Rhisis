using Ether.Network.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Login.Core.Packets;
using Rhisis.Network.Core;
using Sylver.HandlerInvoker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Login.Core
{
    /// <summary>
    /// Defines the core server mechanism.
    /// </summary>
    internal class CoreServer : NetServer<CoreServerClient>, ICoreServer
    {
        private readonly ILogger<CoreServer> _logger;
        private readonly ISCConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates a new <see cref="CoreServer"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="configuration">Core server configuration.</param>
        /// <param name="serviceProvider">Service provider.</param>
        public CoreServer(ILogger<CoreServer> logger, IOptions<ISCConfiguration> configuration, IServiceProvider serviceProvider)
        {
            this._logger = logger;
            this._configuration = configuration.Value;
            this._serviceProvider = serviceProvider;
            this.Configuration.Host = this._configuration.Host;
            this.Configuration.Port = this._configuration.Port;
            this.Configuration.MaximumNumberOfConnections = 10;
            this.Configuration.BufferSize = 128;
            this.Configuration.Backlog = 50;
            this.Configuration.Blocking = false;
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            this._logger.LogInformation($"{nameof(CoreServer)} started!");
        }

        /// <inheritdoc />
        protected override void OnClientConnected(CoreServerClient connection)
        {
            var corePacketFactory = this._serviceProvider.GetRequiredService<ICorePacketFactory>();

            connection.Initialize(this._serviceProvider.GetRequiredService<ILogger<CoreServerClient>>(),
                this._serviceProvider.GetRequiredService<IHandlerInvoker>());

            corePacketFactory.SendWelcome(connection);

            this._logger.LogTrace($"New incoming Core client connection from {connection.RemoteEndPoint}.");
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(CoreServerClient connection)
        {
            switch (connection.ServerInfo)
            {
                case ClusterServerInfo cluster:
                    this._logger.LogInformation($"Cluster server '{cluster.Name}' disconnected from core server.");
                    break;
                case WorldServerInfo world:
                    this._logger.LogInformation($"World server '{world.Name}' disconnected from core server.");
                    break;
                default:
                    this._logger.LogInformation("Unknown server disconnected from core server.");
                    break;
            }
        }

        /// <inheritdoc />
        protected override void OnError(Exception exception)
        {
            this._logger.LogError(exception, $"An error occured in {nameof(CoreServer)}.");
        }

        /// <inheritdoc />
        public CoreServerClient GetClusterServer(int clusterId)
        {
            return this.Clients.FirstOrDefault(x => x.ServerInfo is ClusterServerInfo cluster && cluster.Id == clusterId);
        }

        /// <inheritdoc />
        public bool HasCluster(int clusterId) => this.GetClusterServer(clusterId) != null;

        /// <inheritdoc />
        public CoreServerClient GetWorldServer(int parentClusterId, int worldId)
        {
            return this.Clients.FirstOrDefault(x => x.ServerInfo is WorldServerInfo world && 
                world.ParentClusterId == parentClusterId && world.Id == worldId);
        }

        /// <inheritdoc />
        public bool HasWorld(int parentClusterId, int worldId) => this.GetWorldServer(parentClusterId, worldId) != null;

        /// <inheritdoc />
        public IEnumerable<ClusterServerInfo> GetConnectedClusters() 
            => this.Clients.Where(x => x.ServerInfo.GetType() == typeof(ClusterServerInfo)).Select(x => x.ServerInfo as ClusterServerInfo);
    }
}
