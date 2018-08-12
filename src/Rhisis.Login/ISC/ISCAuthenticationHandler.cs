using Ether.Network.Packets;
using NLog;
using Rhisis.Core.ISC;
using Rhisis.Core.ISC.Packets;
using Rhisis.Core.ISC.Structures;
using Rhisis.Core.Network;
using Rhisis.Login.ISC.Packets;

namespace Rhisis.Login.ISC
{
    public static class ISCAuthenticationHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        [PacketHandler(InterPacketType.Authentication)]
        public static void OnAuthenticate(ISCClient client, INetPacketStream packet)
        {
            var id = packet.Read<int>();
            var host = packet.Read<string>();
            var name = packet.Read<string>();
            var type = packet.Read<byte>();
            client.Type = (InterServerType)type;

            if (client.Type == InterServerType.Cluster)
            {
                if (client.IcsServer.HasClusterWithId(id))
                {
                    Logger.Warn("Server '{0}' disconnected. Reason: Cluster already exists.", name);
                    PacketFactory.SendAuthenticationResult(client, InterServerCode.AUTH_FAILED_CLUSTER_EXISTS);
                    client.IcsServer.DisconnectClient(client.Id);
                }

                client.ServerInfo = new ClusterServerInfo(id, host, name);
                
                PacketFactory.SendAuthenticationResult(client, InterServerCode.AUTH_SUCCESS);
                Logger.Info("Cluster Server '{0}' connected to InterServer.", name);
            }
            else if (client.Type == InterServerType.World)
            {
                var clusterId = packet.Read<int>();
                // TODO: read more informations about world server if needed

                if (!client.IcsServer.HasClusterWithId(clusterId))
                {
                    // No cluster for this server
                    Logger.Warn("Cluster Server with id: '{0}' doesn't exists for World Server '{1}'", clusterId, name);
                    PacketFactory.SendAuthenticationResult(client, InterServerCode.AUTH_FAILED_NO_CLUSTER);
                    client.IcsServer.DisconnectClient(client.Id);
                }

                ISCClient cluster = client.IcsServer.GetCluster(clusterId);
                var clusterInfo = cluster.GetServerInfo<ClusterServerInfo>();

                if (client.IcsServer.HasWorldInCluster(clusterId, id))
                {
                    // World already exists in cluster
                    Logger.Warn("World Server '{0}' already exists in Cluster '{1}'", name, clusterInfo.Name);
                    PacketFactory.SendAuthenticationResult(client, InterServerCode.AUTH_FAILED_WORLD_EXISTS);
                    client.IcsServer.DisconnectClient(client.Id);
                }

                client.ServerInfo = new WorldServerInfo(id, host, name, clusterId);
                clusterInfo.Worlds.Add(client.ServerInfo as WorldServerInfo);
                PacketFactory.SendAuthenticationResult(client, InterServerCode.AUTH_SUCCESS);
                PacketFactory.SendUpdateWorldList(cluster, clusterInfo.Worlds);
                Logger.Info("World Server '{0}' connected to Cluster '{1}'.", name, clusterInfo.Name);
            }
            else
            {
                PacketFactory.SendAuthenticationResult(client, InterServerCode.AUTH_FAILED_UNKNOW_SERVER);
                client.IcsServer.DisconnectClient(client.Id);
            }
        }
    }
}
