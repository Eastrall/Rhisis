using Rhisis.Core.Network;
using Rhisis.Core.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendAddFriendRequest(IPlayerEntity fromEntity, IPlayerEntity toEntity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(toEntity.Id, SnapshotType.ADDFRIENDREQEST);
                packet.Write(fromEntity.Id);
                packet.Write(fromEntity.VisualAppearance.Gender);
                packet.Write(0); // Job
                packet.Write(fromEntity.Object.Name);

                toEntity.Connection.Send(packet);
            }
        }
    }
}