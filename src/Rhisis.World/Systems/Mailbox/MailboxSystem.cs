using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.Database.Entities;
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
                    WorldPacketFactory.SendAddDefinedText(player, DefineText.TID_GAME_MAILBOX_FULL, receiver.Name);
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
                        if (neededGold >= player.PlayerData.Gold)
                        {
                            WorldPacketFactory.SendAddDiagText(player, textClient["TID_GAME_LACKMONEY"]);
                            return;
                        }

                    }
                    catch (OverflowException) // Catch integer overflows to prevent exploits
                    {
                        WorldPacketFactory.SendAddDiagText(player, textClient["TID_GAME_LACKMONEY"]);
                        return;
                    }
                }

                // Calculate item quantity and do all kinds of checks
                DbItem item = null;
                var inventoryItem = player.Inventory.Items[e.ItemSlot];
                if (inventoryItem.Id > -1)
                {
                    var quantity = e.ItemQuantity;
                    if (e.ItemQuantity > inventoryItem.Quantity)
                        quantity = (short)inventoryItem.Quantity;
                    item = database.Items.Get(x => x.Id == inventoryItem.DbId);

                    // TODO: Add the following checks
                    /* All AddDiagText
                     IsUsableItem - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7387
                     IsEquipped - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7392
                     IsQuestItem - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7397
                     IsBound - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7402
                     IsUsing - TID_GAME_CANNOT_DO_USINGITEM  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7407
                     Parts == PARTS_RIDE && ItemJob == JOB_VAGRANT - TID_GAME_CANNOT_POST  https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7424
                     IsCharged() (is this v15? recheck) - TID_GAME_CANNOT_POST https://github.com/domz1/SourceFlyFF/blob/ce4897376fb9949fea768165c898c3e17c84607c/Program/WORLDSERVER/DPSrvr.cpp#L7434
                     */

                    if (inventoryItem.Data.ItemKind3 == ItemKind3.CLOAK /*&& inventoryItem.GuildId != 0*/)
                    {
                        WorldPacketFactory.SendAddDiagText(player, textClient["TID_GAME_CANNOT_POST"]);
                        return;
                    }

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

                // Remove gold now
                player.PlayerData.Gold -= (int)neededGold;

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
                WorldPacketFactory.SendPostMail(player, mail);
            }

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
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                var mail = database.Mails.Get(x => x.Id == e.MailId);
                if (mail.Receiver.Id != player.PlayerData.Id)
                    return;
                mail.IsDeleted = true;
                database.Complete();
                WorldPacketFactory.SendRemoveMail(player, mail);
            }
        }

        private void GetMailItem(IPlayerEntity player, QueryGetMailItemEventArgs e)
        {
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                var mail = database.Mails.Get(x => x.Id == e.MailId);
                if (mail.Receiver.Id == player.PlayerData.Id)
                    return;

                if (mail.HasReceivedItem)
                    return;

                if (!player.Inventory.HasAvailableSlots())
                {
                    WorldPacketFactory.SendAddDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                    return;
                }
                
                mail.HasReceivedItem = true;
                int availableSlot = player.Inventory.GetAvailableSlot();
                player.Inventory.Items[availableSlot] = new Item(mail.Item);
                database.Complete();
                var worldConfiguration = DependencyContainer.Instance.Resolve<WorldConfiguration>();
                WorldPacketFactory.SendGetMailItem(player, mail, worldConfiguration.Id); 
            }
        }

        private void GetMailGold(IPlayerEntity player, QueryGetMailGoldEventArgs e)
        {
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                var mail = database.Mails.Get(x => x.Id == e.MailId);
                if (mail.Receiver.Id != player.PlayerData.Id)
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
                        return;
                    }
                }
                database.Complete();
                var worldConfiguration = DependencyContainer.Instance.Resolve<WorldConfiguration>();
                WorldPacketFactory.SendGetMailGold(player, mail, worldConfiguration.Id); 
            }
        }

        private void ReadMail(IPlayerEntity player, ReadMailEventArgs e)
        {
            using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
            {
                var mail = database.Mails.Get(x => x.Id == e.MailId);
                if (mail.Receiver.Id != player.PlayerData.Id)
                    return;
                mail.HasBeenRead = true;
                database.Complete();
                WorldPacketFactory.SendReadMail(player, mail);
            }
        }
    }
}
