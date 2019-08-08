using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class NpcShopPacketFactory : INpcShopPacketFactory
    {
        /// <inheritdoc />
        public void SendOpenNpcShop(IPlayerEntity player, INpcEntity npc)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(npc.Id, SnapshotType.OPENSHOPWND);

                foreach (ItemContainerComponent shopTab in npc.Shop)
                    shopTab.Serialize(packet);

                player.Connection.Send(packet);
            }
        }
    }
}
