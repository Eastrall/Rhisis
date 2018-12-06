using System;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Entities;
using Rhisis.World.Systems.Mailbox;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        /// <summary>
        /// Shows a window with a custom message at the client.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="text"></param>
        public static void SendAddDiagText(IPlayerEntity player, string text)
        {
            using (var packet = new FFPacket())
            {
                if (MailboxSystem.TEXT_TYPE == TextType.TEXT_DIAG)
                {
                    packet.StartNewMergedPacket(0, SnapshotType.TEXT);
                    packet.Write((byte)TextType.TEXT_DIAG);
                }
                else
                    packet.StartNewMergedPacket(0, SnapshotType.DIAG_TEXT);

                packet.Write(text);
                player.Connection.Send(packet);
            }
        }

        /// <summary>
        /// Shows a defined text at the client.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="textId"></param>
        public static void SendAddDefinedText(IPlayerEntity player, int textId)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.DEFINEDTEXT1);
                packet.Write(textId);
                player.Connection.Send(packet);
            }
        }

        /// <summary>
        /// Shows a defined text at the client and replaces parameter in the string
        /// </summary>
        /// <param name="player"></param>
        /// <param name="textId"></param>
        /// <param name="stringParameter"></param>
        public static void SendAddDefinedText(IPlayerEntity player, int textId, params string[] stringParameter)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.DEFINEDTEXT);
                packet.Write(textId);
                packet.Write(String.Join(" ", stringParameter));
                player.Connection.Send(packet);
            }
        }
    }
}
