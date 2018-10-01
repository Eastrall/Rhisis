using Ether.Network.Packets;
using Rhisis.Core.Structures;
using Rhisis.World.Packets;

namespace Rhisis.World.Handlers
{
    public static class MovementHandler
    {
        public static void OnSnapshotSetDestPosition(WorldClient client, INetPacketStream packet)
        {
            var x = packet.Read<float>();
            var y = packet.Read<float>();
            var z = packet.Read<float>();
            var forward = packet.Read<byte>();

            client.Player.MovableComponent.DestinationPosition = new Vector3(x, y, z);
            client.Player.Object.Angle = Vector3.AngleBetween(client.Player.Object.Position, client.Player.MovableComponent.DestinationPosition);
            client.Player.Follow.Target = null;

            WorldPacketFactory.SendDestinationPosition(client.Player);
        }
    }
}
