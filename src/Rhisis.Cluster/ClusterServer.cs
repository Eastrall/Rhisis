using Ether.Network.Packets;
using Ether.Network.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rhisis.Cluster.Client;
using Rhisis.Cluster.Packets;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Network;
using Rhisis.Network.Core;
using Sylver.HandlerInvoker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Cluster
{
    /// <summary>
    /// Cluster server.
    /// </summary>
    public class ClusterServer : NetServer<ClusterClient>, IClusterServer
    {
        private const int MaxConnections = 500;
        private const int ClientBufferSize = 128;
        private const int ClientBacklog = 50;
        private readonly ILogger<ClusterServer> _logger;
        private readonly IGameResources _gameResources;
        private readonly IServiceProvider _serviceProvider;

        /// <inheritdoc />
        public ClusterConfiguration ClusterConfiguration { get; }

        /// <inheritdoc />
        public IList<WorldServerInfo> WorldServers { get; } = new List<WorldServerInfo>();

        /// <inheritdoc />
        protected override IPacketProcessor PacketProcessor { get; } = new FlyffPacketProcessor();

        /// <summary>
        /// Creates a new <see cref="ClusterServer"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="clusterConfiguration">Cluster Server configuration.</param>
        /// <param name="gameResources">Game resources.</param>
        /// <param name="serviceProvider">Service provider.</param>
        public ClusterServer(ILogger<ClusterServer> logger, IOptions<ClusterConfiguration> clusterConfiguration, IGameResources gameResources, IServiceProvider serviceProvider)
        {
            this._logger = logger;
            this.ClusterConfiguration = clusterConfiguration.Value;
            this._gameResources = gameResources;
            this._serviceProvider = serviceProvider;
            this.Configuration.Host = this.ClusterConfiguration.Host;
            this.Configuration.Port = this.ClusterConfiguration.Port;
            this.Configuration.MaximumNumberOfConnections = MaxConnections;
            this.Configuration.Backlog = ClientBacklog;
            this.Configuration.BufferSize = ClientBufferSize;
            this.Configuration.Blocking = false;
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            this._gameResources.Load(typeof(DefineLoader), typeof(JobLoader));
            this._logger.LogInformation($"'{this.ClusterConfiguration.Name}' cluster server is started and listen on {this.Configuration.Host}:{this.Configuration.Port}."); 
        }

        /// <inheritdoc />
        protected override void OnClientConnected(ClusterClient client)
        {
            this._logger.LogInformation($"New client connected to {nameof(ClusterServer)} from {client.RemoteEndPoint}.");

            client.Initialize(this,
                this._serviceProvider.GetRequiredService<ILogger<ClusterClient>>(),
                this._serviceProvider.GetRequiredService<IHandlerInvoker>(),
                this._serviceProvider.GetRequiredService<IClusterPacketFactory>());
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(ClusterClient client) 
            => this._logger.LogInformation($"Client disconnected from {client.RemoteEndPoint}.");

        /// <inheritdoc />
        protected override void OnError(Exception exception) 
            => this._logger.LogInformation($"{nameof(ClusterServer)} socket error: {exception.Message}");

        /// <inheritdoc />
        public WorldServerInfo GetWorldServerById(int id) => this.WorldServers.FirstOrDefault(x => x.Id == id);
    }
}
