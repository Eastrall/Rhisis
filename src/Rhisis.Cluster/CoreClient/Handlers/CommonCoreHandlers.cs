using Ether.Network.Packets;
using Microsoft.Extensions.Logging;
using Rhisis.Cluster.CoreClient.Packets;
using Rhisis.Network.Core;
using Sylver.HandlerInvoker.Attributes;
using System;

namespace Rhisis.Cluster.CoreClient.Handlers
{
    [Handler]
    public sealed class CommonCoreHandlers
    {
        private readonly ILogger<CommonCoreHandlers> _logger;
        private readonly ICorePacketFactory _corePacketFactory;
        private readonly IClusterServer _clusterServer;

        /// <summary>
        /// Creates a new <see cref="CommonCoreHandlers"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="corePacketFactory">Core packet factory.</param>
        /// <param name="clusterServer">Cluster server instance.</param>
        public CommonCoreHandlers(ILogger<CommonCoreHandlers> logger, ICorePacketFactory corePacketFactory, IClusterServer clusterServer)
        {
            this._logger = logger;
            this._corePacketFactory = corePacketFactory;
            this._clusterServer = clusterServer;
        }

        /// <summary>
        /// On welcome packet received.
        /// </summary>
        /// <param name="client"></param>
        [HandlerAction(CorePacketType.Welcome)]
        public void OnWelcome(IClusterCoreClient client)
        {
            this._corePacketFactory.SendAuthentication(client, this._clusterServer.ClusterConfiguration);
        }

        /// <summary>
        /// Handles the authentication result to the core server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        [HandlerAction(CorePacketType.AuthenticationResult)]
        public void OnAuthenticationResult(IClusterCoreClient client, INetPacketStream packet)
        {
            var authenticationResult = (CoreAuthenticationResultType)(packet.Read<uint>());

            switch (authenticationResult)
            {
                case CoreAuthenticationResultType.Success:
                    this._logger.LogInformation("Cluster Core client authenticated succesfully.");
                    return;
                case CoreAuthenticationResultType.FailedClusterExists:
                    this._logger.LogCritical("Unable to authenticate Cluster Core client. Reason: an other Cluster server (with the same id) is already connected.");
                    break;
                case CoreAuthenticationResultType.FailedUnknownServer:
                    this._logger.LogCritical("Unable to authenticate Cluster Core client. Reason: ISC server doesn't recognize this server. You probably have to update all servers.");
                    break;
                default:
                    this._logger.LogTrace("Core authentification result: {0}", authenticationResult);
                    this._logger.LogCritical("Unable to authenticate Cluster Core client. Reason: Cannot recognize Core server. You probably have to update all servers.");
                    break;
            }

            if (Core.Common.OperatingSystem.IsWindows())
                Console.ReadLine();

            Environment.Exit((int)authenticationResult);
        }

        /// <summary>
        /// Updates the cluster's world servers list.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        [HandlerAction(CorePacketType.UpdateClusterWorldsList)]
        public void OnUpdateClusterWorldsList(IClusterCoreClient client, INetPacketStream packet)
        {
            int numberOfWorldServers = packet.Read<int>();

            this._clusterServer.WorldServers.Clear();

            for (var i = 0; i < numberOfWorldServers; ++i)
            {
                int serverId = packet.Read<int>();
                string serverHost = packet.Read<string>();
                string serverName = packet.Read<string>();
                int serverPort = packet.Read<int>();
                int parentClusterId = packet.Read<int>();

                if (parentClusterId != this._clusterServer.ClusterConfiguration.Id)
                {
                    this._logger.LogCritical($"Cannot add server '{serverName}' to current cluster. Ids doesn't match.");
                    continue;
                }

                var worldServer = new WorldServerInfo(serverId, serverName, serverHost, serverPort, parentClusterId);

                this._clusterServer.WorldServers.Add(worldServer);
            }
        }
    }
}
