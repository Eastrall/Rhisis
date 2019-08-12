using System.Collections.Generic;
using Rhisis.Core.Data;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;
using Rhisis.Core.Structures.Game.Dialogs;
using Rhisis.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class NpcDialogPacketFactory : INpcDialogPacketFactory
    {
        /// <inheritdoc />
        public void SendDialog(IPlayerEntity player, IEnumerable<string> dialogTexts, IEnumerable<DialogLink> dialogLinks)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.RUNSCRIPTFUNC);
                packet.Write((short)DialogOptions.FUNCTYPE_REMOVEALLKEY);

                foreach (var text in dialogTexts)
                {
                    packet.StartNewMergedPacket(player.Id, SnapshotType.RUNSCRIPTFUNC);
                    packet.Write((short)DialogOptions.FUNCTYPE_SAY);
                    packet.Write(text);
                    packet.Write(0); // quest id
                }

                foreach (DialogLink link in dialogLinks)
                {
                    packet.StartNewMergedPacket(player.Id, SnapshotType.RUNSCRIPTFUNC);
                    packet.Write((short)DialogOptions.FUNCTYPE_ADDKEY);
                    packet.Write(link.Title);
                    packet.Write(link.Id);
                    packet.Write(0);
                    packet.Write(0);
                }

                player.Connection.Send(packet);
            }
        }

        /// <inheritdoc />
        public void SendCloseDialog(IPlayerEntity player)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.RUNSCRIPTFUNC);
                packet.Write((short)DialogOptions.FUNCTYPE_EXIT);

                player.Connection.Send(packet);
            }
        }
    }
}
