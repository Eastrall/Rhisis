using Ether.Network.Packets;
using NLog;
using Rhisis.Core.ISC.Packets;
using Rhisis.Core.ISC.Structures;
using Rhisis.Core.Network;

namespace Rhisis.Cluster.ISC
{
    public static class ISCHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        [PacketHandler(InterPacketType.Welcome)]
        public static void OnWelcome(ISCClient client, INetPacketStream packet)
        {
            ISCPackets.SendAuthentication(client, client.ClusterConfiguration.Id, client.ClusterConfiguration.Host, client.ClusterConfiguration.Name);
        }

        [PacketHandler(InterPacketType.AuthenticationResult)]
        public static void OnAuthenticationResult(ISCClient client, INetPacketStream packet)
        {
            var authenticationResult = packet.Read<uint>();

            Logger.Debug("Authentication result: {0}", (InterServerCode)authenticationResult);
        }

        [PacketHandler(InterPacketType.UpdateClusterWorldsList)]
        public static void OnUpdateClusterWorldsList(ISCClient client, INetPacketStream packet)
        {
            client.Worlds.Clear();
            var worldsCount = packet.Read<int>();

            for (int i = 0; i < worldsCount; i++)
            {
                var worldId = packet.Read<int>();
                var worldHost = packet.Read<string>();
                var worldName = packet.Read<string>();
                var worldParentClusterId = packet.Read<int>();

                if (worldParentClusterId != client.ClusterConfiguration.Id)
                    continue;

                client.Worlds.Add(new WorldServerInfo(worldId, worldHost, worldName, worldParentClusterId));
            }
        }
    }
}
