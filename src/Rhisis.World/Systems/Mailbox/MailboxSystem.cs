using System;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Mailbox.EventArgs;

namespace Rhisis.World.Systems.Mailbox
{
    [System(SystemType.Notifiable)]
    public class MailboxSystem : ISystem
    {
        private static readonly ILogger Logger = DependencyContainer.Instance.Resolve<ILogger<MailboxSystem>>();

        public static readonly int MaxMails = 50;
        public static readonly TextType TextType = TextType.TEXT_DIAG;

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity playerEntity))
                return;

            if (!e.CheckArguments())
            {
                Logger.LogError($"Cannot execute mailbox action: {e.GetType()} due to invalid arguments.");
                return;
            }

            switch(e)
            {
                case QueryMailboxEventArgs queryMailboxEvent:
                    GetMails(playerEntity, queryMailboxEvent);
                    break;
                case QueryPostMailEventArgs queryPostMailEvent:
                    SendMail(playerEntity, queryPostMailEvent);
                    break;
                case QueryRemoveMailEventArgs queryRemoveMailEvent:
                    RemoveMail(playerEntity, queryRemoveMailEvent);
                    break;
                case QueryGetMailItemEventArgs queryGetMailItemEvent:
                    GetMailItem(playerEntity, queryGetMailItemEvent);
                    break;
                case QueryGetMailGoldEventArgs queryGetMailGoldEvent:
                    GetMailGold(playerEntity, queryGetMailGoldEvent);
                    break;
                case ReadMailEventArgs readMailEvent:
                    ReadMail(playerEntity, readMailEvent);
                    break;
            }
        }

        private void GetMails(IPlayerEntity player, QueryMailboxEventArgs e)
        {
            WorldPacketFactory.SendMailbox(player);
        }

        private void SendMail(IPlayerEntity player, QueryPostMailEventArgs e)
        {
            // TODO: If mailbox is too far away: return;

            var neededGold = 500; // should be a config value
            var receiverMailQuantity = 0;
            DbCharacter receiver = null;
            DbCharacter sender = null;
            DbItem item = null;

            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                receiver = database.Characters.Get(x => x.Name == e.Receiver);
                if (receiver is null)
                {
                    WorldPacketFactory.SendAddDiagText(player, "TID_MAIL_UNKNOW"); // Get text of TID_MAIL_UNKNOW here
                    return;
                }
                sender = database.Characters.Get(x => x.Id == player.PlayerData.Id);
                receiverMailQuantity = database.Mails.Count(x => x.Receiver == receiver);

                // Receiver and sender is same person
                if (receiver == sender)
                {
                    WorldPacketFactory.SendAddDiagText(player, "TID_GAME_MSGSELFSENDERROR"); // Get text of TID_GAME_MSGSELFSENDERROR here
                    return;
                }

                // Mailbox is full
                if (receiverMailQuantity >= MaxMails)
                {
                    WorldPacketFactory.SendAddDefinedText(player, DefineText.TID_GAME_MAILBOX_FULL, receiver.Name);
                    return;
                }

                // Calculate gold amount
                if (e.Gold < 0)
                {
                    WorldPacketFactory.SendAddDiagText(player, "TID_GAME_LACKMONEY"); // Get text of TID_GAME_LACKMONEY here
                    return;
                }

                checked
                {
                    try
                    {
                        neededGold += e.Gold;
                        if (neededGold >= player.PlayerData.Gold)
                        {
                            WorldPacketFactory.SendAddDiagText(player, "TID_GAME_LACKMONEY"); // Get text of TID_GAME_LACKMONEY here
                            return;
                        }

                    }
                    catch (OverflowException) // Catch integer overflows to prevent exploits
                    {
                        WorldPacketFactory.SendAddDiagText(player, "TID_GAME_LACKMONEY"); // Get text of TID_GAME_LACKMONEY here
                        return;
                    }
                }


                // Calculate item quantity and do all kinds of checks
                var inventoryItem = player.Inventory.Items[e.ItemSlot];
                if (inventoryItem.Id > -1)
                {
                    var quantity = e.ItemQuantity;
                    if (e.ItemQuantity > inventoryItem.Quantity)
                        quantity = (short)inventoryItem.Quantity;
                    item = database.Items.Get(x => x.Id == inventoryItem.Id);

                    // TODO: Add the following checks
                    /* All AddDiagText
                     IsUsableItem - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7387
                     IsEquipped - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7392
                     IsQuestItem - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7397
                     IsBound - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7402
                     IsUsing - TID_GAME_CANNOT_DO_USINGITEM  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7407
                     ItemKind3 == IK3_CLOAK && ItemGuildId != 0 - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7413
                     Parts == PARTS_RIDE && ItemJob == JOB_VAGRANT - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7424
                     IsCharged() (is this v15? recheck) - TID_GAME_CANNOT_POST https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7434
                     */

                    if (inventoryItem.Data.IsStackable)
                    {
                        var futureQuantity = inventoryItem.Quantity - quantity;
                        if (futureQuantity == 0)
                            player.Inventory.Items.Remove(inventoryItem);
                        inventoryItem.Quantity = futureQuantity;
                    }
                    else // Not stackable so always remove it
                        player.Inventory.Items.Remove(inventoryItem);
                }
            }

            // Remove gold now
            player.PlayerData.Gold -= neededGold;

            // Create mail
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                database.Mails.Create(new DbMail
                {
                    Sender = sender,
                    Receiver = receiver,
                    Gold = e.Gold,
                    Item = item,
                    ItemQuantity = e.ItemQuantity,
                    Title = e.Title,
                    Text = e.Text,
                    HasBeenRead = false
                });
                database.Complete();
            }

            WorldPacketFactory.SendMailbox(player);

            // Send message to receiver when he's online
            var worldServer = DependencyContainer.Instance.Resolve<IWorldServer>();
            var receiverEntity = worldServer.GetPlayerEntity(e.Receiver);
            if (receiverEntity != null)
            {
                // set receive player flag newmail
                // send flags packet
                // packet 0x00d3
            }
        }

        private void RemoveMail(IPlayerEntity player, QueryRemoveMailEventArgs e)
        {
            // Delete mail

            // I think we need to send QueryRemoveMail back to the client https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7505
        }

        private void GetMailItem(IPlayerEntity player, QueryGetMailItemEventArgs e)
        {
            // Remove item from mail

            // I think we need to send QueryGetMailItem back to the client https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7563
        }

        private void GetMailGold(IPlayerEntity player, QueryGetMailGoldEventArgs e)
        {
            // Remove money from mail

            // I think we need to send QueryGetMailGold back to the client https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7581
        }

        private void ReadMail(IPlayerEntity player, ReadMailEventArgs e)
        {
            // Set mail to read

            // I think we need to send QueryReadMail packet back to the client https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7609
        }
    }
}
