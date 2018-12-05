﻿using System;
using NLog;
using Rhisis.Core.DependencyInjection;
using Rhisis.Database;
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
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity playerEntity))
                return;

            if (!e.CheckArguments())
            {
                Logger.Error("Cannot execute mailbox action: {0} due to invalid arguments.", e.GetType());
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
                    GetMailMoney(playerEntity, queryGetMailGoldEvent);
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
            var worldServer = DependencyContainer.Instance.Resolve<IWorldServer>();
            var receiverEntity = worldServer.GetPlayerEntity(e.Receiver);

            var receiverId = 0;
            Game.Structures.Item item = null;
            var itemQuantity = 0;
            var neededGold = 500; // should be a config value

            // Receiver is offline
            if (receiverEntity is null)
            {
                var database = DependencyContainer.Instance.Resolve<IDatabase>();
                var dbCharacter = database.Characters.Get(x => x.Name == e.Receiver);
                if (dbCharacter is null)
                    return; // Is there an error packet?
                receiverId = dbCharacter.Id;
            }
            else // Receiver is online
            {
                receiverId = receiverEntity.PlayerData.Id;
            }

            if (e.ItemSlot < InventorySystem.InventorySize)
            {
                item = player.Inventory.Items[e.ItemSlot];
                if (e.ItemQuantity > item.Quantity)
                    return; // Is there an error packet?

                itemQuantity = item.Quantity;
            }
            else // Itemslot is not a valid inventory slot
            {
                return; // Is there an error packet?
            }
            

            checked
            {
                try
                {
                    neededGold += e.Gold;
                    if (neededGold >= player.PlayerData.Gold)
                        return; // Is there an error packet?
                }
                catch (OverflowException) // Catch integer overflows
                {
                    return; // Is there an error packet?
                }
            }

            player.PlayerData.Gold -= neededGold;

            if (item.Data.IsStackable)
            {
                var futureQuantity = item.Quantity - e.ItemQuantity;
                if (futureQuantity == 0)
                    player.Inventory.Items.Remove(item);
                item.Quantity = futureQuantity;
            }
            else
            {
                player.Inventory.Items.Remove(item);
            }
            item.ExtraUsed = 0; // wth?

            // create mail

            // Send message to receiver when he's online
            if (receiverEntity != null)
            {
                // set receive player flag newmail
                // send flags packet // packet 0x00d3
            }

            WorldPacketFactory.SendMailbox(player);
        }

        private void RemoveMail(IPlayerEntity player, QueryRemoveMailEventArgs e)
        {
            // Delete mail, no packet back needed
        }

        private void GetMailItem(IPlayerEntity player, QueryGetMailItemEventArgs e)
        {
            // Remove item from mail, probably send got item packet back
        }

        private void GetMailMoney(IPlayerEntity player, QueryGetMailGoldEventArgs e)
        {
            // Remove money from mail, probably send got money packet back
        }

        private void ReadMail(IPlayerEntity player, ReadMailEventArgs e)
        {
            // Set mail to read, no packet back needed
        }
    }
}
