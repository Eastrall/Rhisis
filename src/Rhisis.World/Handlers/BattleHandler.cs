using Ether.Network.Packets;
using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Battle;
using Rhisis.World.Systems.Follow;
using Rhisis.World.Systems.Inventory;

namespace Rhisis.World.Handlers
{
    public class BattleHandler
    {
        private static readonly ILogger<BattleHandler> Logger = DependencyContainer.Instance.Resolve<ILogger<BattleHandler>>();

        [PacketHandler(PacketType.MELEE_ATTACK)]
        public static void OnMeleeAttack(WorldClient client, INetPacketStream packet)
        {
            var meleePacket = new MeleeAttackPacket(packet);
            var target = client.Player.Object.CurrentLayer.FindEntity<IMonsterEntity>(meleePacket.ObjectId);

            if (target == null)
            {
                Logger.LogError($"Cannot find target with object id {meleePacket.ObjectId}");
                return;
            }

            if (!(target is ILivingEntity))
            {
                Logger.LogError($"Target '{target.Object.Name}' is not a living entity.");
                return;
            }

            Item weaponItem = client.Player.Inventory[InventorySystem.RightWeaponSlot];

            if (weaponItem != null && weaponItem.Data.AttackSpeed != meleePacket.WeaponAttackSpeed)
            {
                Logger.LogCritical($"Player {client.Player.Object.Name} has changed his weapon speed.");
                return;
            }

            client.Player.NotifySystem<BattleSystem>(new MeleeAttackEventArgs(meleePacket.AttackMessage, target, meleePacket.WeaponAttackSpeed));
        }
    }
}
