using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using System;
using Rhisis.Core.Data;
using Rhisis.Core.Exceptions;
using Rhisis.World.Game.Components;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Trade.EventArgs;
using NLog;
using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Trade
{
    [System(SystemType.Notifiable)]
    internal sealed class TradeSystem : ISystem
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Maximum trading items per player
        /// </summary>
        public const int MaxTrade = 25;

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs e)
        {
            if (!e.GetCheckArguments() || !(entity is IPlayerEntity playerEntity))
                return;

            switch (e)
            {
                case TradeRequestEventArgs tradeRequestEventArgs:
                    TradeRequest(playerEntity, tradeRequestEventArgs);
                    break;
                case TradeRequestCancelEventArgs tradeRequestCancelEventArgs:
                    TradeRequestCancel(playerEntity, tradeRequestCancelEventArgs);
                    break;
                case TradeBeginEventArgs tradeBeginEventArgs:
                    Trade(playerEntity, tradeBeginEventArgs);
                    break;
                case TradePutEventArgs tradePutEventArgs:
                    PutItem(playerEntity, tradePutEventArgs);
                    break;
                case TradePutGoldEventArgs tradePutGoldEventArgs:
                    PutGold(playerEntity, tradePutGoldEventArgs);
                    break;
                case TradeCancelEventArgs tradeCancelEventArgs:
                    TradeCancel(playerEntity, tradeCancelEventArgs);
                    break;
                case TradeConfirmEventArgs tradeConfirmEventArgs:
                    TradeConfirm(playerEntity, tradeConfirmEventArgs);
                    break;
                case TradeOkEventArgs tradeOkEventArgs:
                    TradeOk(playerEntity, tradeOkEventArgs);
                    break;
                default:
                    Logger.Warn("Unknown trade action type: {0} for player {1}", e.GetType(), entity.Object.Name);
                    break;
            }
        }

        /// <summary>
        /// Send a new trade request
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private static void TradeRequest(IPlayerEntity player, TradeRequestEventArgs e)
        {
            Logger.Debug("Trade request");

            if (e.TargetId == player.Id)
            {
                throw new RhisisSystemException($"Can't start a Trade with ourselve ({player.Object.Name})");
            }

            if (IsTrading(player))
            {
                throw new RhisisSystemException($"Can't start a Trade when one is already in progress ({player.Object.Name})");
            }

            var target = GetEntityFromContextOf(player, e.TargetId);
            if (IsTrading(target))
            {
                throw new RhisisSystemException($"Can't start a Trade when one is already in progress ({target.Object.Name})");
            }

            WorldPacketFactory.SendTradeRequest(target, player.Id);
        }

        /// <summary>
        /// Cancel/deny a trade request
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private static void TradeRequestCancel(IPlayerEntity player, TradeRequestCancelEventArgs e)
        {
            Logger.Debug("Trade request cancel");

            if (e.TargetId == player.Id)
            {
                throw new RhisisSystemException($"Can't cancel a Trade with ourselve ({player.Object.Name})");
            }

            var target = GetEntityFromContextOf(player, e.TargetId);
            WorldPacketFactory.SendTradeRequestCancel(target, player.Id);
        }

        /// <summary>
        /// Start a new trade / accepting the trade
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private static void Trade(IPlayerEntity player, TradeBeginEventArgs e)
        {
            if (e.TargetId == player.Id)
            {
                throw new RhisisSystemException($"Can't start a Trade with ourselve ({player.Object.Name})");
            }

            if (IsTrading(player))
            {
                throw new RhisisSystemException($"Can't start a Trade when one is already in progress ({player.Object.Name})");
            }

            var target = GetEntityFromContextOf(player, e.TargetId);
            if (IsTrading(target))
            {
                throw new RhisisSystemException($"Can't start a Trade when one is already in progress ({target.Object.Name})");
            }

            player.Trade.TargetId = target.Id;
            target.Trade.TargetId = player.Id;

            WorldPacketFactory.SendTrade(player, target, player.Id);
            WorldPacketFactory.SendTrade(target, player, player.Id);
        }

        /// <summary>
        /// Put an item to the current trade
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private static void PutItem(IPlayerEntity player, TradePutEventArgs e)
        {
            Logger.Debug("Trade PutItem");

            if (IsNotTrading(player))
            {
                throw new RhisisSystemException($"No trade target {player.Object.Name}");
            }
            
            var target = GetEntityFromContextOf(player, player.Trade.TargetId);
            if (IsNotTrading(target))
            {
                CancelTrade(player);
                throw new RhisisSystemException($"Target is not trading {target.Object.Name}");
            }

            if (IsNotTradeState(player, TradeComponent.TradeState.Item) ||
                IsNotTradeState(target, TradeComponent.TradeState.Item))
            {
                throw new RhisisSystemException($"Not the right trade state {player.Object.Name}");
            }

            var item = player.Inventory.GetItem(e.ItemId);
            if (item == null)
            {
                throw new ArgumentException($"TradeSystem: Cannot find item with unique id: {e.ItemId}");
            }

            if (e.Count > item.Quantity)
            {
                throw new ArgumentException($"TradeSystem: More quantity than available for: {e.ItemId}");
            }

            var slotItem = player.Trade.Items[e.Slot];
            if (slotItem != null && slotItem.Id != -1)
            {
                return;
            }

            item.ExtraUsed = e.Count;
            player.Trade.Items[e.Slot] = item;
            player.Trade.ItemCount++;
            WorldPacketFactory.SendTradePut(player, player.Id, e.Slot, e.ItemType, e.ItemId, e.Count);
            WorldPacketFactory.SendTradePut(target, player.Id, e.Slot, e.ItemType, e.ItemId, e.Count);
        }

        /// <summary>
        /// Put gold to the current trade
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private static void PutGold(IPlayerEntity player, TradePutGoldEventArgs e)
        {
            Logger.Debug("PutGold");

            if (IsNotTrading(player))
            {
                throw new RhisisSystemException($"No trade target {player.Object.Name}");
            }

            var target = GetEntityFromContextOf(player, player.Trade.TargetId);
            if (IsNotTrading(target))
            {
                CancelTrade(player);
                throw new RhisisSystemException($"Target is not trading {target.Object.Name}");
            }

            if (IsNotTradeState(player, TradeComponent.TradeState.Item) ||
                IsNotTradeState(target, TradeComponent.TradeState.Item))
            {
                throw new RhisisSystemException($"Not the right trade state {player.Object.Name}");
            }

            player.PlayerData.Gold -= e.Gold;
            player.Trade.Gold += e.Gold;

            WorldPacketFactory.SendTradePutGold(player, player.Id, player.Trade.Gold);
            WorldPacketFactory.SendTradePutGold(target, player.Id, player.Trade.Gold);
        }

        /// <summary>
        /// Cancel the current trade
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private static void TradeCancel(IPlayerEntity player, TradeCancelEventArgs e)
        {
            Logger.Debug("Trade cancel");

            if (IsNotTrading(player))
            {
                throw new RhisisSystemException($"No trade target {player.Object.Name}");
            }

            var target = GetEntityFromContextOf(player, player.Trade.TargetId);
            if (IsNotTrading(target))
            {
                throw new RhisisSystemException($"Target is not trading {target.Object.Name}");
            }

            CancelTrade(player, target, e.Mode);
        }

        /// <summary>
        /// Accept to validate trade
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private static void TradeOk(IPlayerEntity player, TradeOkEventArgs e)
        {
            Logger.Debug("Trade ok");

            if (IsNotTrading(player))
            {
                throw new RhisisSystemException($"No trade target {player.Object.Name}");
            }

            var target = GetEntityFromContextOf(player, player.Trade.TargetId);
            if (IsNotTrading(target))
            {
                CancelTrade(player);
                throw new RhisisSystemException($"Target is not trading {target.Object.Name}");
            }

            if (IsTradeState(player, TradeComponent.TradeState.Item))
            {
                player.Trade.State = TradeComponent.TradeState.Ok;
            }

            if (IsTradeState(target, TradeComponent.TradeState.Ok))
            {
                WorldPacketFactory.SendTradeLastConfirm(player);
                WorldPacketFactory.SendTradeLastConfirm(target);
            }
            else
            {
                WorldPacketFactory.SendTradeOk(player, player.Id);
                WorldPacketFactory.SendTradeOk(target, player.Id);
            }
        }

        private static void TradeConfirm(IPlayerEntity player, TradeConfirmEventArgs e)
        {
            Logger.Debug("Trade confirm");

            if (IsNotTrading(player))
            {
                throw new RhisisSystemException($"No trade target {player.Object.Name}");
            }

            var target = GetEntityFromContextOf(player, player.Trade.TargetId);
            if (IsNotTrading(target))
            {
                CancelTrade(player);
                throw new RhisisSystemException($"Target is not trading {target.Object.Name}");
            }

            if (IsNotTradeState(player, TradeComponent.TradeState.Ok))
            {
                return;
            }

            if (IsTradeState(target, TradeComponent.TradeState.Ok))
            {
                player.Trade.State = TradeComponent.TradeState.Confirm;

                WorldPacketFactory.SendTradeLastConfirmOk(player, player.Id);
                WorldPacketFactory.SendTradeLastConfirmOk(target, player.Id);
            }
            else if (IsTradeState(target, TradeComponent.TradeState.Confirm))
            {
                FinalizeTrade(player, target);
            }
        }

        /// <summary>
        /// Get the specified entity from the player's context
        /// Throw if cannot find it
        /// </summary>
        /// <exception cref="RhisisSystemException"></exception>
        /// <param name="player"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static IPlayerEntity GetEntityFromContextOf(IWorldEntity player, uint id) =>
            player.FindEntity<IPlayerEntity>(id) ??
            throw new RhisisSystemException($"Can't find entity of id {id}");

        private static bool IsTrading(IPlayerEntity entity) =>
            entity.Trade.TargetId != 0;

        private static bool IsNotTrading(IPlayerEntity entity) =>
            entity.Trade.TargetId == 0;

        private static bool IsTradeState(IPlayerEntity player, TradeComponent.TradeState state) =>
            player.Trade.State == state;

        private static bool IsNotTradeState(IPlayerEntity player, TradeComponent.TradeState state) =>
            player.Trade.State != state;

        private static void ResetTrade(IPlayerEntity player) =>
            player.Trade = new TradeComponent();

        /// <summary>
        /// Remove pending gold and send it back
        /// </summary>
        /// <param name="player"></param>
        private static void RefundPlayer(IPlayerEntity player)
        {
            player.PlayerData.Gold += player.Trade.Gold;
            player.Trade.Gold = 0;
            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);
        }

        private static void CancelTrade(IPlayerEntity player, int mode = 0)
        {
            RefundPlayer(player);
            ResetTrade(player);
            WorldPacketFactory.SendTradeCancel(player, player.Id, mode);
        }

        private static void CancelTrade(IPlayerEntity player, IPlayerEntity target, int mode = 0)
        {
            CancelTrade(player, mode);
            CancelTrade(target, mode);
        }

        /// <summary>
        /// Check if player has enough money and if target can receive it
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static TradeComponent.TradeConfirm CheckTradeGold(IPlayerEntity player, IPlayerEntity target)
        {
            var playerTradeGold = player.Trade.Gold;
            var targetTradeGold = target.Trade.Gold;
            var playerGold = player.PlayerData.Gold;
            var targetGold = target.PlayerData.Gold;

            if (playerGold < playerTradeGold ||
                targetGold < targetTradeGold ||
                targetGold + playerTradeGold < 0 ||
                playerGold + targetTradeGold < 0)
                return TradeComponent.TradeConfirm.Error;

            return TradeComponent.TradeConfirm.Ok;
        }

        /// <summary>
        /// Process gold check & transfer
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static bool FinalizeTradeGold(IPlayerEntity player, IPlayerEntity target)
        {
            if (player.Trade.Gold == 0 && target.Trade.Gold == 0)
            {
                return true;
            }

            var tradeResultConfirm = CheckTradeGold(player, target);
            if (tradeResultConfirm == TradeComponent.TradeConfirm.Error)
            {
                CancelTrade(player, target);
                return false;
            }

            player.PlayerData.Gold += target.Trade.Gold;
            target.PlayerData.Gold += player.Trade.Gold;
            return true;
        }

        /// <summary>
        /// Transfer items
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        private static void ProcessItemTransfer(IPlayerEntity player, IPlayerEntity target)
        {
            for (var i = 0; i < MaxTrade; ++i)
            {
                var item = player.Trade.Items.Items[i];
                if (item == null || item.Slot == -1)
                {
                    continue;
                }

                var newItem = item.Clone();
                newItem.CreatorId = -1;

                if (item.Data.IsStackable)
                {
                    var futureQuantity = item.Quantity - item.ExtraUsed;
                    if (futureQuantity < 0)
                    {
                        continue;
                    }

                    if (futureQuantity == 0)
                    {
                        player.Inventory.Items.Remove(item);
                    }
                    
                    item.Quantity = futureQuantity;
                    newItem.Quantity = item.ExtraUsed;
                    item.ExtraUsed = 0;
                    newItem.ExtraUsed = 0;

                    target.Inventory.CreateItem(newItem);
                }
                else
                {
                    player.Inventory.Items.Remove(item);
                    item.ExtraUsed = 0;
                    
                    target.Inventory.CreateItem(newItem);
                }
            }
        }

        /// <summary>
        /// Process items check & transfer
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static bool FinalizeTradeItems(IPlayerEntity player, IPlayerEntity target)
        {
            if (player.Trade.ItemCount > 0 &&
                (!target.Inventory.HasAvailableSlots() ||
                 (InventorySystemOld.InventorySize - target.Inventory.GetItemCount()) < player.Trade.ItemCount))
            {
                CancelTrade(player, target);
                return false;
            }

            if (target.Trade.ItemCount > 0 &&
                (!player.Inventory.HasAvailableSlots() ||
                 (InventorySystemOld.InventorySize - player.Inventory.GetItemCount()) < target.Trade.ItemCount))
            {
                CancelTrade(player, target);
                return false;
            }

            ProcessItemTransfer(player, target);
            ProcessItemTransfer(target, player);
            return true;
        }

        /// <summary>
        /// Start the trade process and finalize it
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        private static void FinalizeTrade(IPlayerEntity player, IPlayerEntity target)
        {
            if (!FinalizeTradeGold(player, target) ||
                !FinalizeTradeItems(player, target))
            {
                Logger.Warn($"Can't finalize trade between {player.Object.Name} and {target.Object.Name}");
                CancelTrade(player, target);
                return;
            }

            // TODO : Save traders
            
            ResetTrade(player);
            ResetTrade(target);
            WorldPacketFactory.SendTradeConsent(player);
            WorldPacketFactory.SendTradeConsent(target);
        }
    }
}
