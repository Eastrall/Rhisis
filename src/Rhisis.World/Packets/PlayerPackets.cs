using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        /// <summary>
        /// Sends player revival packet.
        /// </summary>
        /// <param name="entity"></param>
        public static void SendPlayerRevival(IWorldEntity entity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.REVIVAL_TO_LODESTAR);

                SendToVisible(packet, entity, sendToPlayer: true);
            }
        }

        public static void SendReplaceObject(IPlayerEntity player)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.REPLACE);
                packet.Write(player.Object.MapId);
                packet.Write(player.Object.Position.X);
                packet.Write(player.Object.Position.Y);
                packet.Write(player.Object.Position.Z);

                player.Connection.Send(packet);
            }
        }

        /// <summary>
        /// Send player update level.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="level"></param>
        public static void SendPlayerSetLevel(IPlayerEntity player, int level)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.SETLEVEL);
                packet.Write((short)level);

                SendToVisible(packet, player);
            }
        }

        /// <summary>
        /// Send Player experience.
        /// </summary>
        /// <param name="player"></param>
        public static void SendPlayerExperience(IPlayerEntity player)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.SETEXPERIENCE);
                packet.Write(player.PlayerData.Experience);
                packet.Write((short)player.Object.Level);
                packet.Write(0);
                packet.Write((int)player.Statistics.SkillPoints);
                packet.Write(long.MaxValue); // death exp
                packet.Write((short)player.PlayerData.DeathLevel);

                player.Connection.Send(packet);
            }
        }
    }
}
