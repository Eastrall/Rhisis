using Ether.Network.Server;
using NLog;
using Rhisis.Core.ISC;
using Rhisis.Core.ISC.Structures;
using Rhisis.Core.Network;
using Rhisis.Core.Structures.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Login.ISC
{
    public sealed class ISCServer : NetServer<ISCClient>
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the list of the connected clusters.
        /// </summary>
        public IEnumerable<ClusterServerInfo> Clusters => from x in this.Clients
                                                          where x.ServerInfo is ClusterServerInfo
                                                          select x.ServerInfo as ClusterServerInfo;

        /// <summary>
        /// Creates a new <see cref="ISCServer"/> instance.
        /// </summary>
        /// <param name="configuration"></param>
        public ISCServer(ISCConfiguration configuration)
        {
            this.Configuration.Host = configuration.Host;
            this.Configuration.Port = configuration.Port;
            this.Configuration.MaximumNumberOfConnections = 100;
            this.Configuration.Backlog = 100;
            this.Configuration.BufferSize = 1024;
            this.Configuration.Blocking = false;
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            PacketHandler<ISCClient>.Initialize();
            Logger.Info("InterServer is up.");
        }

        /// <inheritdoc />
        protected override void OnClientConnected(ISCClient connection)
        {
            connection.Initialize(this);
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(ISCClient connection)
        {
            if (string.IsNullOrEmpty(connection.ServerInfo?.Name))
                Logger.Info("Unknown server disconnected from InterServer.");
            else
            {
                Logger.Info("Server '{0}' disconnected from InterServer.", connection.ServerInfo.Name);
                connection.Disconnect();
            }
        }

        /// <inheritdoc />
        protected override void OnError(Exception exception)
        {
            Logger.Error("ISC error: {0}", exception.Message);
        }

        /// <summary>
        /// Check if a cluster is connected.
        /// </summary>
        /// <param name="id">Cluster id</param>
        /// <returns></returns>
        internal bool HasClusterWithId(int id)
        {
            return this.Clients.Any(x => x.ServerInfo is ClusterServerInfo && x.ServerInfo.Id == id);
        }

        /// <summary>
        /// Gets the cluster by his id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal ISCClient GetCluster(int id)
        {
            return (from x in this.Clients
                    where x.ServerInfo is ClusterServerInfo
                    where x.ServerInfo.Id == id
                    select x).FirstOrDefault();
        }

        /// <summary>
        /// Check if a World is inside a cluster.
        /// </summary>
        /// <param name="clusterId">Cluster Id</param>
        /// <param name="worldId">World Id</param>
        /// <returns></returns>
        internal bool HasWorldInCluster(int clusterId, int worldId)
        {
            ISCClient cluster = this.GetCluster(clusterId);

            if (cluster == null)
                return false;

            var clusterInfo = cluster.GetServerInfo<ClusterServerInfo>();

            return clusterInfo.Worlds.Any(x => x.Id == worldId);
        }

        /// <summary>
        /// Gets a world by his id and parent cluster id.
        /// </summary>
        /// <param name="parentClusterId">Parent cluster Id</param>
        /// <param name="worldId">World id</param>
        /// <returns></returns>
        internal ISCClient GetWorld(int parentClusterId, int worldId)
        {
            return (from x in this.Clients
                    where x.Type == InterServerType.World
                    let worldInfo = x.ServerInfo as WorldServerInfo
                    where worldInfo.ParentClusterId == parentClusterId
                    where worldInfo.Id == worldId
                    select x).FirstOrDefault();
        }
    }
}
