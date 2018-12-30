using NLog;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Loaders;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.NpcShop.EventArgs;
using System;

namespace Rhisis.World.Systems.NpcShop
{
    [System(SystemType.Notifiable)]
    public class NpcShopSystem : ISystem
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity player) ||
                !e.CheckArguments())
            {
                Logger.Error("NpcShopSystem: Invalid event arguments.");
                return;
            }

            switch (e)
            {
                case NpcShopOpenEventArgs npcShopEventArgs:
                    this.OpenShop(player, npcShopEventArgs);
                    break;
                case NpcShopCloseEventArgs npcShopEventArgs:
                    this.CloseShop(player);
                    break;
                case NpcShopBuyEventArgs npcShopEventArgs:
                    this.BuyItem(player, npcShopEventArgs);
                    break;
                case NpcShopSellEventArgs npcShopEventArgs:
                    this.SellItem(player, npcShopEventArgs);
                    break;
                default:
                    Logger.Warn("Unknown NpcShop action type: {0} for player {1}", e.GetType(), entity.Object.Name);
                    break;
            }
        }

        /// <summary>
        /// Opens the NPC Shop.
        /// </summary>
        /// <param name="player">Player entity</param>
        /// <param name="e"></param>
        private void OpenShop(IPlayerEntity player, NpcShopOpenEventArgs e)
        {
            var npc = player.Context.FindEntity<INpcEntity>(e.NpcObjectId);
            if (npc == null)
            {
                Logger.Error("ShopSystem: Cannot find NPC with object id : {0}", e.NpcObjectId);
                return;
            }

            if (npc.Shop == null)
            {
                Logger.Error("ShopSystem: NPC '{0}' doesn't have a shop.", npc.Object.Name);
                return;
            }
            
            player.PlayerData.CurrentShopName = npc.Object.Name;

            WorldPacketFactory.SendNpcShop(player, npc);
        }

        /// <summary>
        /// Closes the current shop.
        /// </summary>
        /// <param name="player">Player entity</param>
        private void CloseShop(IPlayerEntity player)
        {
            player.PlayerData.CurrentShopName = null;
        }

        /// <summary>
        /// Buys an item from the current shop.
        /// </summary>
        /// <param name="player">Player entity</param>
        /// <param name="e"></param>
        private void BuyItem(IPlayerEntity player, NpcShopBuyEventArgs e)
        {
            var npcLoader = DependencyContainer.Instance.Resolve<NpcLoader>();
            var npcData = npcLoader.GetNpcData(player.PlayerData.CurrentShopName);

            if (npcData == null)
            {
                Logger.Error($"ShopSystem: Cannot find NPC: {player.PlayerData.CurrentShopName}");
                return;
            }

            if (!npcData.HasShop)
            {
                Logger.Error($"ShopSystem: NPC {npcData.Name} doesn't have a shop.");
                return;
            }

            var currentTab = npcData.Shop.Items[e.Tab];

            if (e.Slot < 0 || e.Slot > currentTab.Count - 1)
            {
                Logger.Error($"ShopSystem: Item slot index was out of tab bounds. Slot: {e.Slot}");
                return;
            }

            var shopItem = currentTab[e.Slot];

            if (shopItem.Id != e.ItemId)
            {
                Logger.Error($"ShopSystem: Shop item id doens't match the item id that {player.Object.Name} is trying to buy.");
                return;
            }

            if (player.PlayerData.Gold < e.ItemData.Cost)
            {
                Logger.Info($"ShopSystem: {player.Object.Name} doens't have enough gold to buy item {e.ItemData.Name} at {e.ItemData.Cost}.");
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKMONEY);
                return;
            }

            int quantity = e.Quantity;
            int cost = e.ItemData.Cost;

            if (e.ItemData.IsStackable)
            {
                for (var i = 0; i < InventorySystem.EquipOffset; i++)
                {
                    Item inventoryItem = player.Inventory.Items[i];

                    if (inventoryItem.Id == e.ItemId)
                    {
                        if (inventoryItem.Quantity + quantity > e.ItemData.PackMax)
                        {
                            int boughtQuantity = inventoryItem.Data.PackMax - inventoryItem.Quantity;

                            quantity -= boughtQuantity;
                            inventoryItem.Quantity = inventoryItem.Data.PackMax;
                            player.PlayerData.Gold -= cost * boughtQuantity;
                        }
                        else
                        {
                            inventoryItem.Quantity += quantity;
                            player.PlayerData.Gold -= cost * quantity;
                            quantity = 0;
                        }

                        WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);
                        WorldPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, inventoryItem.UniqueId, inventoryItem.Quantity);
                    }
                }

                if (quantity > 0)
                {
                    if (!player.Inventory.HasAvailableSlots())
                    {
                        WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                    }
                    else
                    {
                        var item = new Item(e.ItemId, quantity, -1);

                        if (player.Inventory.CreateItem(item))
                        {
                            player.PlayerData.Gold -= cost;
                            WorldPacketFactory.SendItemCreation(player, item);
                            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);
                        }
                        else
                        {
                            Logger.Error("ShopSystem: Failed to create item.");
                        }
                    }
                }
            }
            else
            {
                while (quantity > 0)
                {
                    if (!player.Inventory.HasAvailableSlots())
                    {
                        WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKSPACE);
                        break;   
                    }

                    var item = new Item(e.ItemId, 1, -1);

                    if (player.Inventory.CreateItem(item))
                    {
                        player.PlayerData.Gold -= cost;
                        WorldPacketFactory.SendItemCreation(player, item);
                        WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);
                    }
                    else
                    {
                        Logger.Error("ShopSystem: Failed to create item.");
                    }

                    quantity--;   
                }
            }
        }

        /// <summary>
        /// Sells an item to a NPC shop.
        /// </summary>
        /// <param name="player">Player entity</param>
        /// <param name="e"></param>
        private void SellItem(IPlayerEntity player, NpcShopSellEventArgs e)
        {
            Item itemToSell = player.Inventory.GetItem(e.ItemUniqueId);

            if (itemToSell?.Data == null)
                throw new InvalidOperationException($"ShopSystem: Cannot find item with unique id: {e.ItemUniqueId}");

            if (e.Quantity > itemToSell.Data.PackMax)
                throw new InvalidOperationException($"ShopSystem: Cannot sell more items than the pack max value. {e.Quantity} > {itemToSell.Data.PackMax}");

            // TODO: make more checks:
            // is a quest item
            // is sealed to character
            // ect...

            int sellPrice = itemToSell.Data.Cost / 4;

            Logger.Debug("Selling item: '{0}' for {1}", itemToSell.Data.Name, sellPrice);

            player.PlayerData.Gold += sellPrice * e.Quantity;
            itemToSell.Quantity -= e.Quantity;

            WorldPacketFactory.SendItemUpdate(player, UpdateItemType.UI_NUM, itemToSell.UniqueId, itemToSell.Quantity);
            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);

            if (itemToSell.Quantity <= 0)
                itemToSell.Reset();
        }
    }
}
