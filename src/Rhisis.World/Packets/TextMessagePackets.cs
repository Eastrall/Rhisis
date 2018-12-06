using System;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendAddDiagText(IPlayerEntity player, string text)
        {
            using (var packet = new FFPacket())
            {
                // ifdef __S_SERVER_UNIFY
                packet.StartNewMergedPacket(0, SnapshotType.TEXT);
                packet.Write((byte)0x02); //const BYTE TEXT_DIAG = 0x02 https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/_Network/MsgHdr.h#L1583
                // else __S_SERVER_UNIFY
                packet.StartNewMergedPacket(0, SnapshotType.DIAG_TEXT);
                // endif __S_SERVER_UNIFY
                packet.Write(text);
            }
        }

        public static void SendAddDefinedText(IPlayerEntity player, int textId, string formatString, params string[] stringParameter)
        {
            // See https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/User.cpp#L2209
            // We either need to change the texts in textClient.txt.txt or we need to recreate _vsntprintf
            // Maybe this could be used https://www.codeproject.com/Articles/19274/A-printf-implementation-in-C
            // Replacing the formatters could work aswell but would be hacky
            // I found the following formatters in the textClient.txt.txt
            // %d %s %f %05d %.f %.2f %H %M %S %.2d %.1d %1.d %I64d %% %3.0f %2.0f %3.1f %dYR %dMON %dDD %02d %dH %dM %dS %.0f %c %d-M %d-H %dhours %dminutes %dseconds %.2I64d
            /* This is how it would look like in C:
               char buffer[1024];
               va_list args;
               va_start(args, formatString)
               _vsntprintf(buffer, sizeof(buffer)-1, formatString, args);
               va_end(args);
             */
            string buffer = String.Empty; // temp

            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.DEFINEDTEXT);
                packet.Write(textId);
                packet.Write(buffer);
            }
        }
    }
}
