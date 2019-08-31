using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.PlayerData;
using System;
using System.Linq;

namespace Rhisis.World.Systems.NpcShop
{
    [Injectable]
    public sealed class NpcShopSystem : INpcShopSystem
    {
        private readonly ILogger<NpcShopSystem> _logger;
        private readonly IInventorySystem _inventorySystem;
        private readonly IPlayerDataSystem _playerDataSystem;
        private readonly INpcShopPacketFactory _npcShopPacketFactory;

        /// <summary>
        /// Creates a new <see cref="NpcShopSystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="inventorySystem">Inventory System.</param>
        /// <param name="playerDataSystem">Player data system.</param>
        /// <param name="npcShopPacketFactory">Npc shop packet factory.</param>
        public NpcShopSystem(ILogger<NpcShopSystem> logger, IInventorySystem inventorySystem, IPlayerDataSystem playerDataSystem, INpcShopPacketFactory npcShopPacketFactory)
        {
            this._logger = logger;
            this._inventorySystem = inventorySystem;
            this._playerDataSystem = playerDataSystem;
        }

        /// <inheritdoc />
        public void OpenShop(IPlayerEntity player, uint npcObjectId)
        {
            var npc = player.FindEntity<INpcEntity>(npcObjectId);

            if (npc == null)
            {
                this._logger.LogError($"ShopSystem: Cannot find NPC with object id : {npcObjectId}");
                return;
            }

            if (npc.Shop == null)
            {
                this._logger.LogError($"ShopSystem: NPC '{npc.Object.Name}' doesn't have a shop.");
                return;
            }

            player.PlayerData.CurrentShopName = npc.Object.Name;

            this._npcShopPacketFactory.SendOpenNpcShop(player, npc);
        }

        /// <inheritdoc />
        public void CloseShop(IPlayerEntity player)
        {
            player.PlayerData.CurrentShopName = null;
        }

        /// <inheritdoc />
        public void BuyItem(IPlayerEntity player, NpcShopItemInfo shopItemInfo, int quantity)
        {
            var npc = player.FindEntity<INpcEntity>(x => x.Object.Name == player.PlayerData.CurrentShopName);

            if (npc == null)
            {
                this._logger.LogError($"ShopSystem: Cannot find NPC: {player.PlayerData.CurrentShopName}");
                return;
            }

            if (!npc.Data.HasShop)
            {
                this._logger.LogError($"ShopSystem: NPC {npc.Object.Name} doesn't have a shop.");
                return;
            }

            if (shopItemInfo.Tab < 0 || shopItemInfo.Tab >= ShopData.DefaultTabCount)
            {
                this._logger.LogError($"Attempt to buy an item from {npc.Object.Name} shop tab that is out of range.");
                return;
            }

            ItemContainerComponent shopTab = npc.Shop.ElementAt(shopItemInfo.Tab);

            if (shopItemInfo.Slot < 0 || shopItemInfo.Slot > shopTab.Items.Count - 1)
            {
                this._logger.LogError($"ShopSystem: Item slot index was out of tab bounds. Slot: {shopItemInfo.Slot}");
                return;
            }

            Item shopItem = shopTab[shopItemInfo.Slot];

            if (shopItem.Id != shopItemInfo.ItemId)
            {
                this._logger.LogError($"ShopSystem: Shop item id doens't match the item id that {player.Object.Name} is trying to buy.");
                return;
            }

            if (player.PlayerData.Gold < shopItem.Data.Cost)
            {
                this._logger.LogTrace($"ShopSystem: {player.Object.Name} doens't have enough gold to buy item {shopItem.Data.Name} at {shopItem.Data.Cost}.");
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LACKMONEY);
                return;
            }

            int itemsCreatedCount = this._inventorySystem.CreateItem(player, shopItem, quantity);

            if (itemsCreatedCount > 0)
            {
                this._playerDataSystem.DecreaseGold(player, shopItem.Data.Cost * itemsCreatedCount);
            }
        }

        /// <inheritdoc />
        public void SellItem(IPlayerEntity player, int playerItemUniqueId, int quantity)
        {
            Item itemToSell = player.Inventory.GetItem(playerItemUniqueId);

            if (itemToSell == null)
                throw new ArgumentNullException(nameof(itemToSell), $"Cannot find item with unique id: '{playerItemUniqueId}' in '{player.Object.Name}''s inventory.");

            if (itemToSell.Data == null)
                throw new ArgumentNullException($"Cannot find item data for item '{itemToSell.Id}'.");

            if (quantity > itemToSell.Data.PackMax)
                throw new InvalidOperationException($"Cannot sell more items than the pack max value. {quantity} > {itemToSell.Data.PackMax}");

            // TODO: make more checks:
            // is a quest item
            // is sealed to character
            // ect...

            int sellPrice = itemToSell.Data.Cost / 4; // TODO: make this configurable

            this._logger.LogDebug("Selling item: '{0}' for {1}", itemToSell.Data.Name, sellPrice);

            int deletedQuantity = this._inventorySystem.DeleteItem(player, playerItemUniqueId, quantity);

            if (deletedQuantity > 0)
            {
                this._playerDataSystem.IncreaseGold(player, sellPrice * deletedQuantity);
            }
        }
    }
}
