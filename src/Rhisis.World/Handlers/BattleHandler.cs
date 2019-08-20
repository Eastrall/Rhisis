using Microsoft.Extensions.Logging;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Battle;
using Rhisis.World.Systems.Follow;
using Rhisis.World.Systems.Inventory;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.World.Handlers
{
    [Handler]
    public class BattleHandler
    {
        private readonly ILogger<BattleHandler> _logger;
        private readonly IBattleSystem _battleSystem;
        private readonly IFollowSystem _followSystem;

        /// <summary>
        /// Creates a new <see cref="BattleHandler"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="battleSystem">Battle system.</param>
        public BattleHandler(ILogger<BattleHandler> logger, IBattleSystem battleSystem, IFollowSystem followSystem)
        {
            this._logger = logger;
            this._battleSystem = battleSystem;
            this._followSystem = followSystem;
        }

        /// <summary>
        /// On melee attack.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Incoming packet.</param>
        [HandlerAction(PacketType.MELEE_ATTACK)]
        public void OnMeleeAttack(WorldClient client, MeleeAttackPacket packet)
        {
            var target = client.Player.FindEntity<IMonsterEntity>(packet.ObjectId);

            if (target == null)
            {
                this._logger.LogError($"Cannot find target with object id {packet.ObjectId}");
                return;
            }

            Item weaponItem = client.Player.Inventory.GetItem(x => x.Slot == InventorySystem.RightWeaponSlot) ?? InventorySystem.Hand;

            if (weaponItem != null && weaponItem.Data?.AttackSpeed != packet.WeaponAttackSpeed)
            {
                this._logger.LogCritical($"Player {client.Player.Object.Name} has changed his weapon speed.");
                return;
            }

            if (!target.Follow.IsFollowing && target.Type == WorldEntityType.Monster)
            {
                if (target.Moves.SpeedFactor != 2f)
                {
                    target.Moves.SpeedFactor = 2f;
                    WorldPacketFactory.SendSpeedFactor(target, target.Moves.SpeedFactor);
                }

                this._followSystem.Follow(target, client.Player);
            }

            this._battleSystem.MeleeAttack(client.Player, target, packet.AttackMessage, packet.WeaponAttackSpeed);
        }
    }
}
