using Ether.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.Core.Structures;
using Rhisis.World.Packets;
using Rhisis.Core.Data;

namespace Rhisis.World.Handlers
{
    public static class MovementHandler
    {
        public static void OnSnapshotSetDestPosition(WorldClient client, INetPacketStream packet)
        {
            var setDestPositionPacket = new SetDestPositionPacket(packet);

            client.Player.MovableComponent.DestinationPosition = new Vector3(setDestPositionPacket.X, setDestPositionPacket.Y, setDestPositionPacket.Z);
            client.Player.Object.Angle = Vector3.AngleBetween(client.Player.Object.Position, client.Player.MovableComponent.DestinationPosition);
            client.Player.Object.MovingFlags = ObjectState.OBJSTA_FMOVE;
            client.Player.Follow.Reset();

            WorldPacketFactory.SendDestinationPosition(client.Player);
        }
    }
}
