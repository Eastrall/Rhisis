using Rhisis.Core.Data;
using Rhisis.Core.IO;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Inventory.EventArgs;
using Rhisis.World.Systems.Mobility;
using Rhisis.World.Systems.Recovery;
using Rhisis.World.Systems.Recovery.EventArgs;

namespace Rhisis.World.Game.Behaviors
{
    [Behavior(BehaviorType.Player, IsDefault: true)]
    public sealed class DefaultPlayerBehavior : IBehavior
    {
        private readonly IPlayerEntity _player;
        private readonly IMobilitySystem _mobilitySystem;

        public DefaultPlayerBehavior(IPlayerEntity player, IMobilitySystem mobilitySystem)
        {
            this._player = player;
            this._mobilitySystem = mobilitySystem;
        }

        /// <inheritdoc />
        public void Update()
        {
            this._mobilitySystem.CalculatePosition(this._player);
            this.ProcessIdleHeal(this._player);
        }

        /// <inheritdoc />
        public void OnArrived()
        {
            if (this._player.Follow.IsFollowing && this._player.Follow.Target.Type == WorldEntityType.Drop)
            {
                this.PickUpDroppedItem(this._player, this._player.Follow.Target as IItemEntity);
                this._player.Follow.Reset();
            }
        }

        /// <summary>
        /// Verify all conditions to pickup a dropped item.
        /// </summary>
        /// <param name="player">The player trying to pick-up the dropped item.</param>
        /// <param name="droppedItem">The dropped item.</param>
        private void PickUpDroppedItem(IPlayerEntity player, IItemEntity droppedItem)
        {
            // TODO: check if drop belongs to a party.

            if (droppedItem.Drop.HasOwner && droppedItem.Drop.Owner != player)
            {
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_PRIORITYITEMPER, $"\"{droppedItem.Object.Name}\"");
                return;
            }

            if (droppedItem.Drop.IsGold)
            {
                int droppedGoldAmount = droppedItem.Drop.Item.Quantity;
                long gold = player.PlayerData.Gold + droppedGoldAmount;

                if (gold > int.MaxValue || gold < 0) // Check gold overflow
                {
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_TOOMANYMONEY_USE_PERIN);
                    return;
                }
                else
                {
                    player.PlayerData.Gold = (int)gold;
                    WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.GOLD, player.PlayerData.Gold);
                    WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_REAPMONEY, droppedGoldAmount.ToString("###,###,###,###"), gold.ToString("###,###,###,###"));
                }
            }
            else
            {
                var inventoryItemCreationEvent = new InventoryCreateItemEventArgs(droppedItem.Drop.Item.Id, droppedItem.Drop.Item.Quantity, -1, droppedItem.Drop.Item.Refine);
                SystemManager.Instance.Execute<InventorySystemOld>(player, inventoryItemCreationEvent);
                WorldPacketFactory.SendDefinedText(player, DefineText.TID_GAME_REAPITEM, $"\"{droppedItem.Object.Name}\"");
            }

            WorldPacketFactory.SendMotion(player, ObjectMessageType.OBJMSG_PICKUP);
            droppedItem.Delete();
        }

        /// <summary>
        /// Process Idle heal logic when player is not fighting.
        /// </summary>
        /// <param name="player"></param>
        private void ProcessIdleHeal(IPlayerEntity player)
        {
            if (player.Timers.NextHealTime <= Time.TimeInSeconds())
            {
                if (!player.Battle.IsFighting)
                {
                    SystemManager.Instance.Execute<RecoverySystem>(player, new IdleRecoveryEventArgs(isSitted: false));
                }
            }
        }
    }
}
