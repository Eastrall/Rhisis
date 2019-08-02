using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        /// <summary>
        /// Sends a chat packet to all players around the current player.
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="message">Message</param>
        public static void SendChat(IPlayerEntity player, string message)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.CHAT);
                packet.Write(message);

                player.Connection.Send(packet);
                SendToVisible(packet, player);
            }
        }

        public static void SendChatTo(IWorldEntity fromEntity, IPlayerEntity toPlayer, string message)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(fromEntity.Id, SnapshotType.CHAT);
                packet.Write(message);

                toPlayer.Connection.Send(packet);
            }
        }
    }
}
