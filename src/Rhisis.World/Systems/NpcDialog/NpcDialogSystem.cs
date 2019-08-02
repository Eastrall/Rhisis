using NLog;
using Rhisis.Core.Structures.Game;
using Rhisis.Core.Structures.Game.Dialogs;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Systems.NpcDialog
{
    [System(SystemType.Notifiable)]
    public class NpcDialogSystem : ISystem
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity player) || !(e is NpcDialogOpenEventArgs dialogEvent))
            {
                Logger.Error("DialogSystem: Invalid event arguments.");
                return;
            }

            if (!dialogEvent.GetCheckArguments())
            {
                Logger.Error("DialogSystem: Invalid event action arguments.");
                return;
            }

            this.OpenDialog(player, dialogEvent);
        }

        /// <summary>
        /// Open a NPC dialog box.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private void OpenDialog(IPlayerEntity player, NpcDialogOpenEventArgs e)
        {
            var npcEntity = player.Object.CurrentMap?.FindEntity<INpcEntity>(e.NpcObjectId);

            if (npcEntity == null)
            {
                Logger.Error("DialogSystem: Cannot find NPC with id: {0}", e.NpcObjectId);
                return;
            }

            if (!npcEntity.Data.HasDialog)
            {
                Logger.Error("DialogSystem: NPC '{0}' doesn't have a dialog.", npcEntity.Object.Name);
                return;
            }

            IEnumerable<string> dialogTexts = npcEntity.Data.Dialog.IntroText;

            if (!string.IsNullOrEmpty(e.DialogKey))
            {
                if (e.DialogKey == "BYE")
                {
                    WorldPacketFactory.SendChatTo(npcEntity, player, npcEntity.Data.Dialog.ByeText);
                    WorldPacketFactory.SendCloseDialog(player);
                    return;
                }
                else
                {
                    DialogLink dialogLink = npcEntity.Data.Dialog.Links?.FirstOrDefault(x => x.Id == e.DialogKey);

                    if (dialogLink == null)
                    {
                        Logger.Error("DialogSystem: Cannot find dialog key: '{0}' for NPC '{1}'", e.DialogKey, npcEntity.Object.Name);
                        return;
                    }

                    dialogTexts = dialogLink.Texts;
                }
            }

            WorldPacketFactory.SendDialog(player, dialogTexts, npcEntity.Data.Dialog.Links);
        }
    }
}