using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using System;

namespace Rhisis.World.Systems.NpcShop
{
    [Injectable]
    public sealed class NpcShopSystem : INpcShopSystem
    {
        private readonly ILogger<NpcShopSystem> _logger;
        private readonly IGameResources _gameResources;
        private readonly IInventorySystem _inventorySystem;
        private readonly INpcShopPacketFactory _npcShopPacketFactory;

        public NpcShopSystem(ILogger<NpcShopSystem> logger, IGameResources gameResources, IInventorySystem inventorySystem, INpcShopPacketFactory npcShopPacketFactory)
        {
            this._logger = logger;
            this._gameResources = gameResources;
            this._inventorySystem = inventorySystem;
            this._npcShopPacketFactory = npcShopPacketFactory;
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
        public void BuyItem(IPlayerEntity player)
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

            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void SellItem(IPlayerEntity player)
        {
            throw new NotImplementedException();
        }
    }
}
