using Rhisis.Core.Data;
using Rhisis.Core.Structures;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendSpeedFactor(IWorldEntity entity, float speedFactor)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.SET_SPEED_FACTOR);
                packet.Write(speedFactor);

                SendToVisible(packet, entity);
            }
        }

        public static void SendDestinationPosition(IMovableEntity movableEntity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(movableEntity.Id, SnapshotType.DESTPOS);
                packet.Write(movableEntity.Moves.DestinationPosition.X);
                packet.Write(movableEntity.Moves.DestinationPosition.Y);
                packet.Write(movableEntity.Moves.DestinationPosition.Z);
                packet.Write<byte>(1);

                SendToVisible(packet, movableEntity);
            }
        }


        public static void SendDestinationAngle(IWorldEntity entity, bool left)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.DESTANGLE);
                packet.Write(entity.Object.Angle);
                packet.Write(left);

                SendToVisible(packet, entity);
            }
        }
    }
}
