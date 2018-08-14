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
                packet.Write(fromEntity.PlayerData.Id);
                packet.Write(fromEntity.VisualAppearance.Gender);
                packet.Write(0); // Job
                packet.Write(fromEntity.Object.Name);

                toEntity.Connection.Send(packet);
            }
        }

        public static void SendAddFriendCancel(IPlayerEntity fromEntity, IPlayerEntity toEntity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(toEntity.PlayerData.Id, SnapshotType.ADDFRIENDCANCEL);

                toEntity.Connection.Send(packet);
            }
        }

        public static void SendAddFriend(IPlayerEntity toEntity, IPlayerEntity friendEntity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(toEntity.PlayerData.Id, SnapshotType.ADDFRIEND);
                packet.Write(friendEntity.PlayerData.Id);
                packet.Write(friendEntity.Object.Name);

                toEntity.Connection.Send(packet);
            }
        }
    }
}