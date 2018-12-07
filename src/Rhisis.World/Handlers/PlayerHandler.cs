using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Systems.Follow;

namespace Rhisis.World.Handlers
{
    internal static class PlayerHandler
    {
        [PacketHandler(PacketType.PLAYERSETDESTOBJ)]
        public static void OnPlayerSetDestObject(WorldClient client, INetPacketStream packet)
        {
            var targetObjectId = packet.Read<int>();
            var distance = packet.Read<float>();
            var followEvent = new FollowEventArgs(targetObjectId, distance);

            client.Player.NotifySystem<FollowSystem>(followEvent);
        }
    }
}
