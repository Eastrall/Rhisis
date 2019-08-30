using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.PlayerData;
using System;

namespace Rhisis.World.Systems.Trade
{
    /// <summary>
    /// Trade system.
    /// </summary>
    [Injectable]
    public sealed class TradeSystem : ITradeSystem
    {
        /// <summary>
        /// Maximum trading items per player
        /// </summary>
        public const int MaxTrade = 25;

        private readonly ILogger<TradeSystem> _logger;
        private readonly ITradePacketFactory _tradePacketFactory;
        private readonly IPlayerDataSystem _playerDataSystem;
        private readonly IInventorySystem _inventorySystem;

        /// <summary>
        /// Creates a new <see cref="TradeSystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="tradePacketFactory">Trade packet factory.</param>
        /// <param name="playerDataSystem">Player data system.</param>
        /// <param name="inventorySystem">Inventory system.</param>
        public TradeSystem(ILogger<TradeSystem> logger, ITradePacketFactory tradePacketFactory, IPlayerDataSystem playerDataSystem, IInventorySystem inventorySystem)
        {
            this._logger = logger;
            this._tradePacketFactory = tradePacketFactory;
            this._playerDataSystem = playerDataSystem;
            this._inventorySystem = inventorySystem;
        }

        /// <inheritdoc />
        public void Initialize(IPlayerEntity player)
        {
            player.Trade = new TradeComponent(MaxTrade);
        }

        /// <inheritdoc />
        public void RequestTrade(IPlayerEntity player, uint targetObjectId)
        {
            this._logger.LogTrace($"Player '{player.Object.Name}' request trade to {targetObjectId}.");

            if (player.Id == targetObjectId)
            {
                throw new InvalidOperationException($"Can't start a Trade with ourselve ({player.Object.Name})");
            }

            IPlayerEntity target = this.GetTargetEntity(player, targetObjectId);

            this.ThrowIfPlayerTrade(player, isTrading: true);
            this.ThrowIfPlayerTrade(target, isTrading: true);

            this._tradePacketFactory.SendTradeRequest(player, target);
        }

        /// <inheritdoc />
        public void DeclineTradeRequest(IPlayerEntity player, uint targetObjectId)
        {
            this._logger.LogTrace($"Player '{player.Object.Name}' is declining trade.");

            if (player.Id == targetObjectId)
            {
                throw new InvalidOperationException($"Can't decline a Trade with ourselve ({player.Object.Name})");
            }

            IPlayerEntity target = this.GetTargetEntity(player, targetObjectId);

            this._tradePacketFactory.SendTradeRequestCancel(player, target);
        }

        /// <inheritdoc />
        public void StartTrade(IPlayerEntity player, uint targetObjectId)
        {
            if (player.Id == targetObjectId)
            {
                throw new InvalidOperationException($"Can't start trading with ourselve ({player.Object.Name})");
            }

            IPlayerEntity target = this.GetTargetEntity(player, targetObjectId);

            this._logger.LogTrace($"Trade is starting between '{player.Object.Name}' and '{target.Object.Name}'.");

            this.ThrowIfPlayerTrade(player, isTrading: true);
            this.ThrowIfPlayerTrade(target, isTrading: true);

            player.Trade.TargetId = target.Id;
            target.Trade.TargetId = player.Id;

            this._tradePacketFactory.SendTrade(player, target, player.Id);
            this._tradePacketFactory.SendTrade(target, player, player.Id);
        }

        /// <inheritdoc />
        public void PutItem(IPlayerEntity player, int itemUniqueId, int quantity, int itemType, int destinationSlot)
        {
            this._logger.LogTrace($"Player '{player.Object.Name}' is putting item with unique id '{itemUniqueId}' to trade slot '{destinationSlot}'.");
            this.ThrowIfPlayerTrade(player, isTrading: false);

            IPlayerEntity target = this.GetTargetEntity(player, player.Trade.TargetId);

            try
            {
                this.ThrowIfPlayerTrade(target, isTrading: false);
            }
            catch (InvalidOperationException)
            {
                this.CancelTradeAndRefund(player);
                this.CancelTradeAndRefund(target);
                throw;
            }

            if (player.Trade.State != TradeComponent.TradeState.Item || target.Trade.State != TradeComponent.TradeState.Item)
            {
                this._tradePacketFactory.SendTradePutError(player);
                return;
            }

            Item inventoryItem = player.Inventory.GetItem(itemUniqueId);

            if (inventoryItem == null)
            {
                throw new ArgumentNullException($"Cannot find item with unique id '{itemUniqueId}' in '{player.Object.Name}' inventory.');");
            }

            if (!this.IsTradeItemValid(inventoryItem, out DefineText errorText))
            {
                WorldPacketFactory.SendDefinedText(player, errorText);
            }

            int tradingQuantity = Math.Min(quantity, inventoryItem.Quantity);

            if (!player.Trade.Items.IsSlotAvailable(destinationSlot))
            {
                this._logger.LogTrace($"Destination slot '{destinationSlot}' is not available for player '{player.Object.Name}'");
                return;
            }

            inventoryItem.ExtraUsed = tradingQuantity;
            player.Trade.Items[destinationSlot] = inventoryItem;
            player.Trade.ItemCount++;

            this._tradePacketFactory.SendTradePut(player, trader: player, (byte)destinationSlot, (byte)itemType, (byte)inventoryItem.UniqueId, (short)tradingQuantity);
            this._tradePacketFactory.SendTradePut(target, trader: player, (byte)destinationSlot, (byte)itemType, (byte)inventoryItem.UniqueId, (short)tradingQuantity);
        }

        /// <inheritdoc />
        public void PutGold(IPlayerEntity player, int goldAmount)
        {
            this.ThrowIfPlayerTrade(player, isTrading: false);

            IPlayerEntity target = this.GetTargetEntity(player, player.Trade.TargetId);

            try
            {
                this.ThrowIfPlayerTrade(target, isTrading: false);
            }
            catch (InvalidOperationException)
            {
                this.CancelTradeAndRefund(player);
                this.CancelTradeAndRefund(target);
                throw;
            }

            int gold = Math.Min(player.PlayerData.Gold, goldAmount);

            player.PlayerData.Gold -= gold;
            player.Trade.Gold += gold;

            this._tradePacketFactory.SendTradePutGold(player, trader: player, player.Trade.Gold);
            this._tradePacketFactory.SendTradePutGold(target, trader: player, player.Trade.Gold);
        }

        /// <inheritdoc />
        public void CancelTrade(IPlayerEntity player, int mode)
        {
            if (player.Trade.TargetId == 0)
                return;

            IPlayerEntity target = this.GetTargetEntity(player, player.Trade.TargetId);

            this._logger.LogTrace($"Trade canceled between '{player}' and '{target}'.");

            this.ThrowIfPlayerTrade(player, isTrading: false);
            this.ThrowIfPlayerTrade(target, isTrading: false);
            this.CancelTradeAndRefund(player, mode);
            this.CancelTradeAndRefund(target, mode);
        }

        /// <inheritdoc />
        public void ConfirmTrade(IPlayerEntity player)
        {
            IPlayerEntity target = this.GetTargetEntity(player, player.Trade.TargetId);

            this._logger.LogTrace($"Player {player} has confirmed the trade.");

            try
            {
                this.ThrowIfPlayerTrade(player, isTrading: false);
                this.ThrowIfPlayerTrade(target, isTrading: false);
            }
            catch
            {
                this.CancelTradeAndRefund(player);
                this.CancelTradeAndRefund(target);
                throw;
            }

            if (player.Trade.State == TradeComponent.TradeState.Item)
            {
                player.Trade.State = TradeComponent.TradeState.Ok;
            }

            if (target.Trade.State == TradeComponent.TradeState.Ok)
            {
                this._tradePacketFactory.SendTradeLastConfirm(player);
                this._tradePacketFactory.SendTradeLastConfirm(target);
            }
            else
            {
                this._tradePacketFactory.SendTradeOk(player, player.Id);
                this._tradePacketFactory.SendTradeOk(target, player.Id);
            }
        }

        /// <inheritdoc />
        public void LastConfirmTrade(IPlayerEntity player)
        {
            IPlayerEntity target = this.GetTargetEntity(player, player.Trade.TargetId);

            this._logger.LogTrace($"Player {player} has finally confirmed the trade.");

            try
            {
                this.ThrowIfPlayerTrade(player, isTrading: false);
                this.ThrowIfPlayerTrade(target, isTrading: false);
            }
            catch (InvalidOperationException)
            {
                this.CancelTradeAndRefund(player);
                this.CancelTradeAndRefund(target);
                throw;
            }

            if (player.Trade.State == TradeComponent.TradeState.Ok)
            {
                player.Trade.State = TradeComponent.TradeState.Confirm;
                this._tradePacketFactory.SendTradeLastConfirmOk(player, player.Id);
                this._tradePacketFactory.SendTradeLastConfirmOk(target, player.Id);
            }

            if (player.Trade.State == TradeComponent.TradeState.Confirm && target.Trade.State == TradeComponent.TradeState.Confirm)
            {
                if (!FinalizeTradeGold(player, target) || !FinalizeTradeItems(player, target))
                {
                    this._logger.LogWarning($"Can't finalize trade between {player} and {target}");
                    this.CancelTradeAndRefund(player);
                    this.CancelTradeAndRefund(target);
                    return;
                }

                // TODO : Save traders

                player.Trade.Reset();
                target.Trade.Reset();
                this._tradePacketFactory.SendTradeConsent(player);
                this._tradePacketFactory.SendTradeConsent(target);
            }
        }

        /// <summary>
        /// Gets a player entity from the current player's spawn list using the given target object id.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="targetObjectId">Target object id.</param>
        /// <exception cref="ArgumentNullException">If the player has not been found.</exception>
        /// <returns>Player entity if found; throws <see cref="ArgumentNullException"/> otherwise.</returns>
        private IPlayerEntity GetTargetEntity(IPlayerEntity player, uint targetObjectId)
        {
            var target = player.FindEntity<IPlayerEntity>(targetObjectId);

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), $"Cannot find target entity with id '{targetObjectId}'.");
            }

            return target;
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if the player is already trading.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="isTrading"></param>
        /// <exception cref="InvalidOperationException">Player is already trading.</exception>
        private void ThrowIfPlayerTrade(IPlayerEntity player, bool isTrading)
        {
            if (player.Trade.IsTrading == isTrading)
            {
                throw new InvalidOperationException($"Player '{player.Object.Name}' is {(!isTrading ? "not" : "already" )} trading.");
            }
        }

        /// <summary>
        /// Check if the item to trade is valid.
        /// </summary>
        /// <param name="itemToTrade">Item to trade.</param>
        /// <param name="errorText">Output error text if item is not valid.</param>
        /// <returns>True if item is valid; false otherwise.</returns>
        private bool IsTradeItemValid(Item itemToTrade, out DefineText errorText)
        {
            errorText = DefineText.TID_BLANK;

            if (itemToTrade.ExtraUsed != 0)
            {
                errorText = DefineText.TID_GAME_CANNOTTRADE_ITEM;
                return false;
            }

            if (itemToTrade.IsEquipped())
            {
                errorText = DefineText.TID_GAME_CANNOTTRADE_ITEM;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Cancels the trade and refund the player.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="mode">Cancel mode.</param>
        private void CancelTradeAndRefund(IPlayerEntity player, int mode = 0)
        {
            this._playerDataSystem.IncreaseGold(player, player.Trade.Gold);
            player.Trade.Reset();

            this._tradePacketFactory.SendTradeCancel(player, mode);
        }

        /// <summary>
        /// Finalize the gold trade.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="target">Target.</param>
        /// <returns></returns>
        private bool FinalizeTradeGold(IPlayerEntity player, IPlayerEntity target)
        {
            if (player.Trade.Gold == 0 && target.Trade.Gold == 0)
                return true;

            int playerTradeGold = player.Trade.Gold;
            int targetTradeGold = target.Trade.Gold;
            int playerGold = player.PlayerData.Gold;
            int targetGold = target.PlayerData.Gold;

            if (playerGold < playerTradeGold || playerGold + targetTradeGold < 0 ||
                targetGold < targetTradeGold || targetGold + playerTradeGold < 0)
                return false;

            player.PlayerData.Gold += targetTradeGold;
            target.PlayerData.Gold += playerTradeGold;

            return true;
        }

        /// <summary>
        /// Finalize the item trade.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="target">Target.</param>
        /// <returns></returns>
        private bool FinalizeTradeItems(IPlayerEntity player, IPlayerEntity target)
        {
            if (!this.CheckIfPlayerHasEnoughPlace(player, target) && !this.CheckIfPlayerHasEnoughPlace(target, player))
            {
                this.CancelTradeAndRefund(player);
                this.CancelTradeAndRefund(target);
                return false;
            }

            this.ProcessItemTransfer(player, target);
            this.ProcessItemTransfer(target, player);

            return true;
        }

        /// <summary>
        /// Checks if the player has enough place in his inventory.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="target">Target trader.</param>
        /// <returns>True if player has enough space; false otherwise.</returns>
        private bool CheckIfPlayerHasEnoughPlace(IPlayerEntity player, IPlayerEntity target)
        {
            if (player.Trade.ItemCount > 0 &&
                   (!target.Inventory.HasAvailableSlots() ||
                    (InventorySystem.InventorySize - target.Inventory.GetItemCount()) < player.Trade.ItemCount))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Transfers the items between traders.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="target">Target trader.</param>
        private void ProcessItemTransfer(IPlayerEntity player, IPlayerEntity target)
        {
            for (int i = 0; i < MaxTrade; i++)
            {
                Item item = player.Trade.Items.Items[i];

                if (item == null || item.Slot == -1)
                    continue;

                Item newItem = item.Clone();
                int tradeQuantity = item.ExtraUsed;
                int futureQuantity = Math.Max(item.Quantity - tradeQuantity, 0);

                if (futureQuantity <= 0)
                {
                    this._inventorySystem.DeleteItem(player, item.UniqueId, item.ExtraUsed, sendToPlayer: false);
                }

                this._inventorySystem.CreateItem(target, newItem, tradeQuantity, sendToPlayer: false);

                if (futureQuantity > 0)
                {
                    item.Quantity = futureQuantity;
                    item.ExtraUsed = 0;
                }
            }
        }
    }
}
