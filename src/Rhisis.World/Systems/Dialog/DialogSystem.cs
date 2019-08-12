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
        private readonly IChatPacketFactory _chatPacketFactory;
        private readonly INpcDialogPacketFactory _npcDialogPacketFactory;

        /// <summary>
        /// Creates a new <see cref="DialogSystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="chatPacketFactory">Chat packet factory.</param>
        /// <param name="npcDialogPacketFactory">Dialog packet factory.</param>
        public DialogSystem(ILogger<DialogSystem> logger, IChatPacketFactory chatPacketFactory, INpcDialogPacketFactory npcDialogPacketFactory)
        {
            this._logger = logger;
            this._chatPacketFactory = chatPacketFactory;
            this._npcDialogPacketFactory = npcDialogPacketFactory;
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
                    this._chatPacketFactory.SendChatTo(npcEntity, player, npcEntity.Data.Dialog.ByeText);
                    this._npcDialogPacketFactory.SendCloseDialog(player);
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

            this._npcDialogPacketFactory.SendDialog(player, dialogTexts, npcEntity.Data.Dialog.Links);
        }
    }
}
