using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Structures.Game.Dialogs;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Systems.Dialog
{
    [Injectable]
    public sealed class DialogSystem : IDialogSystem
    {
        private readonly ILogger<DialogSystem> _logger;

        public DialogSystem(ILogger<DialogSystem> logger)
        {
            this._logger = logger;
        }

        /// <inheritdoc />
        public void OpenNpcDialog(IPlayerEntity player, uint npcObjectId, string dialogKey)
        {
            var npcEntity = player.FindEntity<INpcEntity>(npcObjectId);

            if (npcEntity == null)
            {
                this._logger.LogError($"Cannot find NPC with id: {npcObjectId}.");
                return;
            }

            if (!npcEntity.Data.HasDialog)
            {
                this._logger.LogError($"NPC '{npcEntity.Object.Name}' doesn't have a dialog.");
                return;
            }

            IEnumerable<string> dialogTexts = npcEntity.Data.Dialog.IntroText;

            if (!string.IsNullOrEmpty(dialogKey))
            {
                if (dialogKey == "BYE")
                {
                    WorldPacketFactory.SendChatTo(npcEntity, player, npcEntity.Data.Dialog.ByeText);
                    WorldPacketFactory.SendCloseDialog(player);
                    return;
                }
                else
                {
                    DialogLink dialogLink = npcEntity.Data.Dialog.Links?.FirstOrDefault(x => x.Id == dialogKey);

                    if (dialogLink == null)
                    {
                        this._logger.LogError($"Cannot find dialog key: '{dialogKey}' for NPC '{npcEntity.Object.Name}'.");
                        return;
                    }

                    dialogTexts = dialogLink.Texts;
                }
            }

            WorldPacketFactory.SendDialog(player, dialogTexts, npcEntity.Data.Dialog.Links);
        }
    }
}
