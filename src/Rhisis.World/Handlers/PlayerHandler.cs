using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Systems.Follow;
using Rhisis.World.Systems.PlayerData;
using Rhisis.World.Systems.PlayerData.EventArgs;

namespace Rhisis.World.Handlers
{
    internal static class PlayerHandler
    {
        [PacketHandler(PacketType.PLAYERSETDESTOBJ)]
        public static void OnPlayerSetDestObject(WorldClient client, INetPacketStream packet)
        {
            var targetObjectId = packet.Read<uint>();
            var distance = packet.Read<float>();
            var followEvent = new FollowEventArgs(targetObjectId, distance);

            client.Player.NotifySystem<FollowSystem>(followEvent);
        }

        [PacketHandler(PacketType.QUERY_PLAYER_DATA)]
        public static void OnQueryPlayerData(WorldClient client, INetPacketStream packet)
        {
            var onQueryPlayerDataPacket = new QueryPlayerDataPacket(packet);
            var queryPlayerDataEvent = new QueryPlayerDataEventArgs(onQueryPlayerDataPacket.PlayerId, onQueryPlayerDataPacket.Version);
            client.Player.NotifySystem<PlayerDataSystem>(queryPlayerDataEvent);
        }

        [PacketHandler(PacketType.QUERY_PLAYER_DATA2)]
        public static void OnQueryPlayerData2(WorldClient client, INetPacketStream packet)
        {
            var onQueryPlayerData2Packet = new QueryPlayerData2Packet(packet);
            var queryPlayerData2Event = new QueryPlayerData2EventArgs(onQueryPlayerData2Packet.Size, onQueryPlayerData2Packet.PlayerDictionary);
            client.Player.NotifySystem<PlayerDataSystem>(queryPlayerData2Event);
        }
    }
}
