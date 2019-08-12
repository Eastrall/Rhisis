using System.Collections.Generic;
using Rhisis.World.Game.Entities;
using Rhisis.Core.Structures.Game.Dialogs;

namespace Rhisis.World.Packets
{
    public interface INpcDialogPacketFactory
    {
        /// <summary>
        /// Sends a NPC dialog to a player.
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="text">Npc dialog text</param>
        /// <param name="dialogLinks">Npc dialog links</param>
        void SendDialog(IPlayerEntity player, IEnumerable<string> dialogTexts, IEnumerable<DialogLink> dialogLinks);

        /// <summary>
        /// Send a packet to close the NPC dialog box.
        /// </summary>
        /// <param name="player"></param>
        void SendCloseDialog(IPlayerEntity player);
    }
}
