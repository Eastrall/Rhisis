using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class InventoryPacketFactory : IInventoryPacketFactory
    {
        private readonly IPacketFactoryUtilities _packetFactoryUtilities;

        /// <summary>
        /// Creates a new <see cref="InventoryPacketFactory"/> instance.
        /// </summary>
        /// <param name="packetFactoryUtilities">Packet factory utilities.</param>
        public InventoryPacketFactory(IPacketFactoryUtilities packetFactoryUtilities)
        {
            this._packetFactoryUtilities = packetFactoryUtilities;
        }

        /// <inheritdoc />
        public void SendItemMove(IPlayerEntity entity, byte sourceSlot, byte destinationSlot)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.MOVEITEM);
                packet.Write<byte>(0); // item type (not used)
                packet.Write(sourceSlot);
                packet.Write(destinationSlot);

                entity.Connection.Send(packet);
            }
        }

        /// <inheritdoc />
        public void SendItemEquip(IPlayerEntity entity, Item item, int targetSlot, bool equip)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.DOEQUIP);
                packet.Write(item.UniqueId);
                packet.Write<byte>(0); // Guild id
                packet.Write(equip ? (byte)0x01 : (byte)0x00);
                packet.Write(item.Id);
                packet.Write(item.Refines);
                packet.Write(0); // flag
                packet.Write(targetSlot);

                this._packetFactoryUtilities.SendToVisible(packet, entity, sendToPlayer: true);
            }
        }

        /// <inheritdoc />
        public void SendItemCreation(IPlayerEntity entity, Item item)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.CREATEITEM);
                packet.Write<byte>(0);
                item.Serialize(packet);
                packet.Write<byte>(1);
                packet.Write((byte)item.UniqueId);
                packet.Write((short)item.Quantity);

                entity.Connection.Send(packet);
            }
        }

        /// <inheritdoc />
        public void SendItemUpdate(IPlayerEntity entity, UpdateItemType updateType, int uniqueId, int value)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.UPDATE_ITEM);
                packet.Write<byte>(0);
                packet.Write((byte)uniqueId);
                packet.Write((byte)updateType);
                packet.Write(value);
                packet.Write(0); // time

                entity.Connection.Send(packet);
            }
        }
    }
}
