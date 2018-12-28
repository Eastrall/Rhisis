using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        /// <summary>
        /// Write data of a single player. Can contain data of multiplate players when filled with send = false.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="playerId"></param>
        /// <param name="name"></param>
        /// <param name="jobId"></param>
        /// <param name="level"></param>
        /// <param name="gender"></param>
        /// <param name="online"></param>
        /// <param name="send">Decides if the packet gets send to the player</param>
        public static void SendPlayerData(IPlayerEntity entity, uint playerId, string name, byte jobId, byte level, byte gender, int version, bool online, bool send = true)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(FFPacket.NullId, SnapshotType.QUERY_PLAYER_DATA);

                packet.Write(playerId);
                packet.Write(name);
                packet.Write(jobId);
                packet.Write(level);
                packet.Write(gender);
                packet.Write((byte)0); // padding
                packet.Write(version);
                packet.Write(online);
                packet.Write((byte)0); // padding
                packet.Write((byte)0); // padding
                packet.Write((byte)0); // padding

                if (send) 
                    entity.Connection.Send(packet);
            }
        }

        public static void SendModifyMode(IPlayerEntity entity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.MODIFYMODE);

                packet.Write(entity.PlayerData.Mode);

                SendToVisible(packet, entity, true);
            }
        }
    }
}
