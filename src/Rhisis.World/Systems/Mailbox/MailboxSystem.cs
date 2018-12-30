using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
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
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                var receiver = database.Characters.Get(x => x.Id == player.PlayerData.Id);
                if (receiver != null)
                    WorldPacketFactory.SendMailbox(player, receiver.ReceivedMails.Where(x => !x.IsDeleted).ToList());
            }
        }

        private void SendMail(IPlayerEntity player, QueryPostMailEventArgs e)
        {
            // TODO: If mailbox is too far away: return;

            var textClient = DependencyContainer.Instance.Resolve<TextClientLoader>();
            var worldConfiguration = DependencyContainer.Instance.Resolve<WorldConfiguration>();
            var neededGold = worldConfiguration.MailShippingCost;
            
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                var receiver = database.Characters.Get(x => x.Name == e.Receiver);

                // Receiver doesn't exist
                if (receiver is null)
                {
                    WorldPacketFactory.SendAddDiagText(player, textClient["TID_MAIL_UNKNOW"]);
                    return;
                }

                var sender = database.Characters.Get(x => x.Id == player.PlayerData.Id);

                // Receiver and sender is same person
                if (receiver == sender)
                {
                    WorldPacketFactory.SendAddDiagText(player, textClient["TID_GAME_MSGSELFSENDERROR"]);
                    return;
                }

                // Mailbox is full
                if (receiver.ReceivedMails.Count(x => !x.IsDeleted) >= MaxMails)
                {
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_MAILBOX_FULL, receiver.Name);
                    return;
                }

                // Calculate gold amount
                if (e.Gold < 0)
                {
                    WorldPacketFactory.SendAddDiagText(player, textClient["TID_GAME_LACKMONEY"]);
                    return;
                }

                checked
                {
                    try
                    {
                        neededGold += e.Gold;
                        if (neededGold > player.PlayerData.Gold)
                        {
                            WorldPacketFactory.SendAddDiagText(player, textClient["TID_GAME_LACKMONEY"]);
                            return;
                        }

                    }
                    catch (OverflowException) // Catch integer overflows to prevent exploits
                    {
                        Logger.LogError($"{player.Object.Name} caused an OverflowException by placing {e.Gold} into a mail.");
                        WorldPacketFactory.SendAddDiagText(player, textClient["TID_GAME_LACKMONEY"]);
                        return;
                    }
                }

                // Calculate item quantity and do all kinds of checks
                DbItem item = null;
                var inventoryItem = player.Inventory.Items.FirstOrDefault(x => x.Slot == e.ItemSlot);
                if (inventoryItem != null && inventoryItem.Id > -1)
                {
                    var quantity = e.ItemQuantity;
                    if (e.ItemQuantity > inventoryItem.Quantity)
                        quantity = (short)inventoryItem.Quantity;
                    item = database.Items.Get(x => x.Id == inventoryItem.DbId);

                    // TODO: Add the following checks
                    /* All AddDiagText
                     IsBound - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7402
                     IsUsing - TID_GAME_CANNOT_DO_USINGITEM  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7407
                     IsCharged()- TID_GAME_CANNOT_POST https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7434
                     */

                    if (inventoryItem.IsEquipped() ||
                        inventoryItem.ExtraUsed != 0 ||
                        inventoryItem.Data.ItemKind3 == ItemKind3.QUEST ||
                        (inventoryItem.Data.ItemKind3 == ItemKind3.CLOAK /*&& inventoryItem.GuildId != 0*/) // || GuildId on items is not yet implemented
                      /*(inventoryItem.Data.Parts == Parts.PARTS_RIDE && inventoryItem.Data.ItemJob == DefineJob.JOB_VAGRANT)*/)
                    {
                        WorldPacketFactory.SendAddDiagText(player, textClient["TID_GAME_CANNOT_POST"]);
                        return;
                    }

                    // TODO: Not yet implemented
                    /*if (inventoryItem.Data.Parts == Parts.PARTS_RIDE && inventoryItem.Data.ItemJob == DefineJob.JOB_VAGRANT)
                    {
                        WorldPacketFactory.SendAddDiagText(player, textClient["TID_GAME_CANNOT_POST"]);
                        return;
                    }*/

                    if (inventoryItem.Data.IsStackable)
                    {
                        var futureQuantity = inventoryItem.Quantity - quantity;
                        if (futureQuantity == 0)
                            player.Inventory.Items.Remove(inventoryItem);
                        inventoryItem.Quantity = futureQuantity;
                        WorldPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, inventoryItem.Slot, futureQuantity);
                    }
                    else // Not stackable so always remove it
                    {
                        player.Inventory.Items.Remove(inventoryItem);
                        WorldPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, inventoryItem.Slot, 0);
                    }
                }

                // Remove gold now
                player.PlayerData.Gold -= (int)neededGold;
                WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);

                // Create mail
                var mail = new DbMail
                {
                    Sender = sender,
                    Receiver = receiver,
                    Gold = e.Gold,
                    Item = item,
                    ItemQuantity = item is null ? (short)0 : e.ItemQuantity,
                    Title = e.Title,
                    Text = e.Text,
                    HasBeenRead = false
                };
                database.Mails.Create(mail);
                database.Complete();
                WorldPacketFactory.SendAddDiagText(player, textClient["TID_MAIL_SEND_OK"]);

                // Send message to receiver when he's online
                var worldServer = DependencyContainer.Instance.Resolve<IWorldServer>();
                var receiverEntity = worldServer.GetPlayerEntity(e.Receiver);
                if (receiverEntity != null)
                {
                    receiverEntity.PlayerData.Mode |= ModeType.MODE_MAILBOX;
                    WorldPacketFactory.SendModifyMode(receiverEntity);
                }
            }
        }

        private void RemoveMail(IPlayerEntity player, QueryRemoveMailEventArgs e)
        {
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                var mail = database.Mails.Get(x => x.Id == e.MailId && x.ReceiverId == player.PlayerData.Id);

                if (mail is null)
                    return;

                mail.IsDeleted = true;
                database.Complete();
                WorldPacketFactory.SendRemoveMail(player, mail, RemovedFromMail.Mail);
            }
        }

        private void GetMailItem(IPlayerEntity player, QueryGetMailItemEventArgs e)
        {
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                var mail = database.Mails.Get(x => x.Id == e.MailId && x.ReceiverId == player.PlayerData.Id);

                if (mail is null)
                    return;

                if (mail.HasReceivedItem)
                    return;

                if (!player.Inventory.HasAvailableSlots())
                {
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                    return;
                }
                
                mail.HasReceivedItem = true;
                int availableSlot = player.Inventory.GetAvailableSlot();
                player.Inventory.Items[availableSlot] = new Item(mail.Item);
                database.Complete();
                WorldPacketFactory.SendRemoveMail(player, mail, RemovedFromMail.Item); 
            }
        }

        private void GetMailGold(IPlayerEntity player, QueryGetMailGoldEventArgs e)
        {
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                var mail = database.Mails.Get(x => x.Id == e.MailId && x.ReceiverId == player.PlayerData.Id);

                if (mail is null)
                    return;

                if (mail.HasReceivedGold)
                    return;

                checked
                {
                    try
                    {
                        player.PlayerData.Gold += (int)mail.Gold;
                        mail.HasReceivedGold = true;
                    }
                    catch (OverflowException)
                    {
                        Logger.LogError($"{player.Object.Name} caused an OverflowException by taking {mail.Gold} out of mail {mail.Id}.");
                        return;
                    }
                }
                database.Complete();
                WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);
                WorldPacketFactory.SendRemoveMail(player, mail, RemovedFromMail.Gold);
            }
        }

        private void ReadMail(IPlayerEntity player, ReadMailEventArgs e)
        {
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                var mail = database.Mails.Get(x => x.Id == e.MailId && x.ReceiverId == player.PlayerData.Id);

                if (mail is null)
                    return;

                var unreadMails = database.Mails.Count(x => !x.HasBeenRead && x.ReceiverId == player.PlayerData.Id) - 1;
                mail.HasBeenRead = true;
                database.Complete();
                WorldPacketFactory.SendRemoveMail(player, mail, RemovedFromMail.Read);
                if (unreadMails == 0 && player.PlayerData.Mode.HasFlag(ModeType.MODE_MAILBOX))
                {
                    player.PlayerData.Mode &= ~ModeType.MODE_MAILBOX;
                    WorldPacketFactory.SendModifyMode(player);
                }
            }
        }
    }
}
