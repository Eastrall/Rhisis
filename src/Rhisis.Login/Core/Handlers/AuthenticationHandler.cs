using Ether.Network.Packets;
using Microsoft.Extensions.Logging;
using Rhisis.Login.Core.Packets;
using Rhisis.Network.Core;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.Login.Core.Handlers
{
    [Handler]
    internal sealed class AuthenticationHandler
    {
        private readonly ILogger<AuthenticationHandler> _logger;
        private readonly ICoreServer _coreServer;
        private readonly ICorePacketFactory _corePacketFactory;

        /// <summary>
        /// Creates a new <see cref="AuthenticationHandler"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="coreServer">Core server instance.</param>
        /// <param name="corePacketFactory">Core server packet factory.</param>
        public AuthenticationHandler(ILogger<AuthenticationHandler> logger, ICoreServer coreServer, ICorePacketFactory corePacketFactory)
        {
            this._logger = logger;
            this._coreServer = coreServer;
            this._corePacketFactory = corePacketFactory;
        }

        /// <summary>
        /// Authenticates a core server client.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet stream.</param>
        [HandlerAction(CorePacketType.Authenticate)]
        public void OnAuthenticate(CoreServerClient client, INetPacketStream packet)
        {
            var id = packet.Read<int>();
            var name = packet.Read<string>();
            var host = packet.Read<string>();
            var port = packet.Read<int>();
            var type = (ServerType)packet.Read<byte>();

            if (type == ServerType.Cluster)
            {
                if (this._coreServer.HasCluster(id))
                {
                    this._logger.LogWarning($"Cluster Server '{name}' incoming connection from {client.RemoteEndPoint} refused. Reason: An other Cluster server with id '{id}' is already connected.");
                    this._corePacketFactory.SendAuthenticationResult(client, CoreAuthenticationResultType.FailedClusterExists);
                    this._coreServer.DisconnectClient(client.Id);
                    return;
                }

                client.ServerInfo = new ClusterServerInfo(id, name, host, port);
                this._logger.LogInformation($"Cluster server '{name}' connected to ISC server from {client.RemoteEndPoint}.");
            }
            else if (type == ServerType.World)
            {
                var parentClusterId = packet.Read<int>();

                CoreServerClient parentClusterServer = this._coreServer.GetClusterServer(parentClusterId);

                if (parentClusterServer == null)
                {
                    this._logger.LogWarning($"World server '{name}' incoming connection from {client.RemoteEndPoint} refused. Reason: Cluster server with id '{parentClusterId}' is not connected.");

                    this._corePacketFactory.SendAuthenticationResult(client, CoreAuthenticationResultType.FailedNoCluster);
                    this._coreServer.DisconnectClient(client.Id);
                    return;
                }

                if (this._coreServer.HasWorld(parentClusterId, id))
                {
                    this._logger.LogWarning($"World server '{name}' incoming connection from {client.RemoteEndPoint} refused. Reason: An other World server with id '{id}' is already connected to Cluster Server '{parentClusterServer.ServerInfo.Name}'.");
                    this._corePacketFactory.SendAuthenticationResult(client, CoreAuthenticationResultType.FailedWorldExists);
                    this._coreServer.DisconnectClient(client.Id);
                    return;
                }

                var cluster = parentClusterServer.ServerInfo as ClusterServerInfo;
                var worldInfo = new WorldServerInfo(id, name, host, port, parentClusterId);

                cluster.Worlds.Add(worldInfo);
                client.ServerInfo = worldInfo;
                this._corePacketFactory.SendUpdateWorldList(parentClusterServer, cluster.Worlds);
                this._logger.LogInformation($"World server '{name}' join cluster '{cluster.Name}' and is connected to ISC server from {client.RemoteEndPoint}.");
            }
            else
            {
                this._logger.LogWarning($"Incoming core connection from {client.RemoteEndPoint} refused. Reason: server type is unknown.");
                this._corePacketFactory.SendAuthenticationResult(client, CoreAuthenticationResultType.FailedUnknownServer);
                this._coreServer.DisconnectClient(client.Id);
                return;
            }

            this._corePacketFactory.SendAuthenticationResult(client, CoreAuthenticationResultType.Success);
        }
    }
}
