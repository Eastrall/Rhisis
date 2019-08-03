using Microsoft.Extensions.Logging;
using Rhisis.Login.Core.Packets;
using Rhisis.Network.Core;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.Login.Core.Handlers
{
    [Handler]
    public sealed class CommonCoreHandler
    {
        private readonly ILogger<CommonCoreHandler> _logger;
        private readonly ICoreServer _coreServer;
        private readonly ICorePacketFactory _corePacketFactory;

        /// <summary>
        /// Creates a new <see cref="CommonCoreHandler"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="coreServer">Core server.</param>
        /// <param name="corePacketFactory">Core packet factory.</param>
        public CommonCoreHandler(ILogger<CommonCoreHandler> logger, ICoreServer coreServer, ICorePacketFactory corePacketFactory)
        {
            this._logger = logger;
            this._coreServer = coreServer;
            this._corePacketFactory = corePacketFactory;
        }

        /// <summary>
        /// Disconnects a client.
        /// </summary>
        /// <param name="client">Client.</param>
        [HandlerAction(CorePacketType.Disconnect)]
        public void OnDisconnect(ICoreServerClient client)
        {
            switch (client.ServerInfo)
            {
                case ClusterServerInfo cluster:
                    this._logger.LogInformation($"Cluster server '{cluster.Name}' disconnected from core server.");
                    cluster.Worlds.Clear();
                    break;
                case WorldServerInfo world:
                    this._logger.LogInformation($"World server '{world.Name}' disconnected from core server.");
                    ICoreServerClient clusterClient = this._coreServer.GetClusterServer(world.ParentClusterId);
                    var clusterServerInfo = clusterClient.ServerInfo as ClusterServerInfo;

                    if (clusterServerInfo != null)
                    {
                        clusterServerInfo.Worlds.Remove(world);
                        this._corePacketFactory.SendUpdateWorldList(clusterClient, clusterServerInfo.Worlds);
                    }

                    break;
                default:
                    this._logger.LogInformation("Unknown server disconnected from core server.");
                    break;
            }
        }
    }
}
