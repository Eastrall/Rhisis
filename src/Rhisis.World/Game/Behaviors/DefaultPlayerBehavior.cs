using Rhisis.Core.Data;
using Rhisis.Core.IO;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Mobility;
using Rhisis.World.Systems.PlayerData;
using Rhisis.World.Systems.Recovery;

namespace Rhisis.World.Game.Behaviors
{
    [Behavior(BehaviorType.Player, IsDefault: true)]
    public sealed class DefaultPlayerBehavior : IBehavior
    {
        private readonly IPlayerEntity _player;
        private readonly IMobilitySystem _mobilitySystem;
        private readonly IInventorySystem _inventorySystem;
        private readonly IPlayerDataSystem _playerDataSystem;
        private readonly IRecoverySystem _recoverySystem;
        private readonly IRegionTriggerSystem _regionTriggerSystem;
        private readonly IMoverPacketFactory _moverPacketFactory;

        /// <summary>
        /// Creates a new <see cref="DefaultPlayerBehavior"/> instance.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="mobilitySystem">Mobility system.</param>
        /// <param name="inventorySystem">Inventory system.</param>
        /// <param name="playerDataSystem">Player data system.</param>
        public DefaultPlayerBehavior(IPlayerEntity player, IMobilitySystem mobilitySystem, IInventorySystem inventorySystem, IPlayerDataSystem playerDataSystem, IRecoverySystem recoverySystem, IRegionTriggerSystem regionTriggerSystem, IMoverPacketFactory moverPacketFactory)
        {
            this._player = player;
            this._mobilitySystem = mobilitySystem;
            this._inventorySystem = inventorySystem;
            this._playerDataSystem = playerDataSystem;
            this._recoverySystem = recoverySystem;
            this._regionTriggerSystem = regionTriggerSystem;
            this._moverPacketFactory = moverPacketFactory;
        }

        /// <inheritdoc />
        public void Update()
        {
            if (!this._player.Object.Spawned || this._player.Health.IsDead)
                return;

            this._mobilitySystem.CalculatePosition(this._player);
            this._regionTriggerSystem.CheckWrapzones(this._player);
            this.ProcessIdleHeal();
        }

        /// <inheritdoc />
        public void OnArrived()
        {
            if (this._player.Follow.IsFollowing && this._player.Follow.Target.Type == WorldEntityType.Drop)
            {
                this.PickUpDroppedItem(this._player.Follow.Target as IItemEntity);
                this._player.Follow.Reset();
            }
        }

        /// <summary>
        /// Verify all conditions to pickup a dropped item.
        /// </summary>
        /// <param name="droppedItem">The dropped item.</param>
        private void PickUpDroppedItem(IItemEntity droppedItem)
        {
            // TODO: check if drop belongs to a party.

            if (droppedItem.Drop.HasOwner && droppedItem.Drop.Owner != this._player)
            {
                WorldPacketFactory.SendDefinedText(this._player, DefineText.TID_GAME_PRIORITYITEMPER, $"\"{droppedItem.Object.Name}\"");
                return;
            }

            if (droppedItem.Drop.IsGold)
            {
                int droppedGoldAmount = droppedItem.Drop.Item.Quantity;

                if (this._playerDataSystem.IncreaseGold(this._player, droppedGoldAmount))
                {
                    WorldPacketFactory.SendDefinedText(this._player, DefineText.TID_GAME_REAPMONEY, droppedGoldAmount.ToString("###,###,###,###"), this._player.PlayerData.Gold.ToString("###,###,###,###"));
                }
            }
            else
            {
                this._inventorySystem.CreateItem(this._player, droppedItem.Drop.Item, droppedItem.Drop.Item.Quantity);
                WorldPacketFactory.SendDefinedText(this._player, DefineText.TID_GAME_REAPITEM, $"\"{droppedItem.Object.Name}\"");
            }

            this._moverPacketFactory.SendMotion(this._player, ObjectMessageType.OBJMSG_PICKUP);
            droppedItem.Delete();
        }

        /// <summary>
        /// Process Idle heal logic when player is not fighting.
        /// </summary>
        /// <param name="player"></param>
        private void ProcessIdleHeal()
        {
            if (this._player.Timers.NextHealTime <= Time.TimeInSeconds())
            {
                if (!this._player.Battle.IsFighting)
                {
                    this._recoverySystem.IdleRecevory(this._player, isSitted: false);
                }
            }
        }
    }
}
