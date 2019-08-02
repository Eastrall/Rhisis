using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Helpers;
using Rhisis.World.Game.Maps;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.SpecialEffect;
using Rhisis.World.Systems.SpecialEffect.EventArgs;
using Rhisis.World.Systems.Teleport;
using System;
using System.Collections.Generic;

namespace Rhisis.World.Systems.Inventory
{
    internal sealed class InventoryItemUsage
    {
        private readonly ILogger<InventoryItemUsage> _logger;
        private readonly IMapManager _mapManager;

        /// <summary>
        /// Creates a new <see cref="InventoryItemUsage"/> instance.
        /// </summary>
        public InventoryItemUsage(ILogger<InventoryItemUsage> logger, IMapManager mapManager)
        {
            this._logger = logger;
            this._mapManager = mapManager;
        }

        /// <summary>
        /// Check if the given item is equipable by a player.
        /// </summary>
        /// <param name="player">Player trying to equip an item.</param>
        /// <param name="item">Item to equip.</param>
        /// <returns>True if the player can equip the item; false otherwise.</returns>
        public bool IsItemEquipable(IPlayerEntity player, Item item)
        {
            if (item.Data.ItemSex != int.MaxValue && item.Data.ItemSex != player.VisualAppearance.Gender)
            {
                this._logger.LogDebug("Wrong sex for armor");
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_WRONGSEX, item.Data.Name);
                return false;
            }

            if (player.Object.Level < item.Data.LimitLevel)
            {
                this._logger.LogDebug("Player level to low");
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_REQLEVEL, item.Data.LimitLevel.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Uses an item from the player's inventory.
        /// </summary>
        /// <param name="player">Player using an item.</param>
        /// <param name="itemToUse">Item to use.</param>
        public void UseItem(IPlayerEntity player, Item itemToUse)
        {
            this._logger.LogTrace($"{player.Object.Name} want to use {itemToUse.Data.Name}.");

            if (player.Inventory.ItemHasCoolTime(itemToUse) && !player.Inventory.CanUseItemWithCoolTime(itemToUse))
            {
                this._logger.LogDebug($"Player '{player.Object.Name}' cannot use item {itemToUse.Data.Name}: CoolTime.");
                return;
            }

            switch (itemToUse.Data.ItemKind2)
            {
                case ItemKind2.REFRESHER:
                case ItemKind2.POTION:
                case ItemKind2.FOOD:
                    this.UseFoodItem(player, itemToUse);
                    break;
                case ItemKind2.BLINKWING:
                    this.UseBlinkwingItem(player, itemToUse);
                    break;
                default:
                    this._logger.LogDebug($"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                    WorldPacketFactory.SendSnoop(player, $"Item usage for {itemToUse.Data.ItemKind2} is not implemented.");
                    break;
            }
        }

        /// <summary>
        /// Use food item.
        /// </summary>
        /// <param name="player">Player using the item.</param>
        /// <param name="foodItemToUse">Food item to use.</param>
        public void UseFoodItem(IPlayerEntity player, Item foodItemToUse)
        {
            foreach (KeyValuePair<DefineAttributes, int> parameter in foodItemToUse.Data.Params)
            {
                if (parameter.Key == DefineAttributes.HP || parameter.Key == DefineAttributes.MP || parameter.Key == DefineAttributes.FP)
                {
                    int currentPoints = PlayerHelper.GetPoints(player, parameter.Key);
                    int maxPoints = PlayerHelper.GetMaxPoints(player, parameter.Key);
                    int itemMaxRecovery = foodItemToUse.Data.AbilityMin;

                    if (parameter.Value >= 0)
                    {
                        if (currentPoints >= itemMaxRecovery)
                        {
                            float limitedRecovery = parameter.Value * 0.3f;

                            if (currentPoints + limitedRecovery > maxPoints)
                            {
                                currentPoints = maxPoints;
                            }
                            else
                            {
                                switch (parameter.Key)
                                {
                                    case DefineAttributes.HP: WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LIMITHP); break;
                                    case DefineAttributes.MP: WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LIMITMP); break;
                                    case DefineAttributes.FP: WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_LIMITFP); break;
                                }

                                currentPoints += (int)limitedRecovery;
                            }
                        }
                        else
                        {
                            currentPoints = Math.Min(currentPoints + parameter.Value, maxPoints);
                        }
                    }

                    PlayerHelper.SetPoints(player, parameter.Key, currentPoints);
                    WorldPacketFactory.SendUpdateAttributes(player, parameter.Key, PlayerHelper.GetPoints(player, parameter.Key));
                }
                else
                {
                    // TODO: food triggers a skill.
                    this._logger.LogWarning($"Activating a skill throught food.");
                    WorldPacketFactory.SendFeatureNotImplemented(player, "skill with food");
                }
            }

            this.DecreaseItem(player, foodItemToUse);
        }

        /// <summary>
        /// Uses blinkwing items.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="blinkwing"></param>
        private void UseBlinkwingItem(IPlayerEntity player, Item blinkwing)
        {
            if (player.Object.Level < blinkwing.Data.LimitLevel)
            {
                this._logger.LogError($"Player {player.Object.Name} cannot use {blinkwing.Data.Name}. Level too low.");
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_USINGNOTLEVEL);
                return;
            }

            // TODO: Check if player is sit
            // TODO: Check if player is on Kebaras island
            // TODO: Check if player is in guild war map

            TeleportEventArgs teleportEvent;

            if (blinkwing.Data.ItemKind3 == ItemKind3.TOWNBLINKWING)
            {
                IMapRevivalRegion revivalRegion = player.Object.CurrentMap.GetNearRevivalRegion(player.Object.Position);

                if (revivalRegion == null)
                {
                    this._logger.LogError($"Cannot find any revival region for map '{player.Object.CurrentMap.Name}'.");
                    return;
                }
                if (player.Object.MapId != revivalRegion.MapId)
                {
                    IMapInstance revivalMap = this._mapManager.GetMap(revivalRegion.MapId);

                    if (revivalMap == null)
                    {
                        this._logger.LogError($"Cannot find revival map with id '{revivalRegion.MapId}'.");
                        player.Connection.Server.DisconnectClient(player.Connection.Id);
                        return;
                    }

                    revivalRegion = revivalMap.GetRevivalRegion(revivalRegion.Key);
                }

                teleportEvent = new TeleportEventArgs(revivalRegion.MapId,
                    revivalRegion.RevivalPosition.X,
                    revivalRegion.RevivalPosition.Z,
                    revivalRegion.RevivalPosition.Y);
            }
            else
            {
                teleportEvent = new TeleportEventArgs(blinkwing.Data.WeaponTypeId, // Map Id
                    blinkwing.Data.ItemAtkOrder1, // X
                    blinkwing.Data.ItemAtkOrder3, // Z
                    blinkwing.Data.ItemAtkOrder2, // Y
                    blinkwing.Data.ItemAtkOrder4); // Angle
            }

            player.Inventory.ItemInUseActionId = player.Delayer.DelayAction(TimeSpan.FromMilliseconds(blinkwing.Data.SkillReadyType), () =>
            {
                SystemManager.Instance.Execute<TeleportSystem>(player, teleportEvent);
                SystemManager.Instance.Execute<SpecialEffectSystem>(player, new SpecialEffectBaseMotionEventArgs(StateModeBaseMotion.BASEMOTION_OFF));
                player.Inventory.ItemInUseActionId = Guid.Empty;
                this.DecreaseItem(player, blinkwing);
            });

            SystemManager.Instance.Execute<SpecialEffectSystem>(player, new SpecialEffectBaseMotionEventArgs(StateModeBaseMotion.BASEMOTION_ON, blinkwing));
        }

        /// <summary>
        /// Decreases an item from player's inventory.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="item">Item to decrease.</param>
        private void DecreaseItem(IPlayerEntity player, Item item)
        {
            var itemUpdateType = UpdateItemType.UI_NUM;

            if (player.Inventory.ItemHasCoolTime(item))
            {
                itemUpdateType = UpdateItemType.UI_COOLTIME;
                player.Inventory.SetCoolTime(item, item.Data.CoolTime);
            }

            if (!item.Data.IsPermanant)
                item.Quantity--;

            WorldPacketFactory.SendItemUpdate(player, itemUpdateType, item.UniqueId, item.Quantity);

            if (item.Data.SfxObject3 != 0)
                SystemManager.Instance.Execute<SpecialEffectSystem>(player, new SpecialEffectEventArgs(item.Data.SfxObject3));
        }
    }
}
