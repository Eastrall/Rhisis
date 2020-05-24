using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using System;

namespace Rhisis.World.Packets.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    internal class InventoryPacketFactory : PacketFactoryBase, IInventoryPacketFactory
    {
        /// <inheritdoc />
        public void SendItemMove(IPlayerEntity entity, byte sourceSlot, byte destinationSlot)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.MOVEITEM);
            packet.Write<byte>(0); // item type (not used)
            packet.Write(sourceSlot);
            packet.Write(destinationSlot);

            SendToPlayer(entity, packet);
        }

        /// <inheritdoc />
        public void SendItemEquip(IPlayerEntity entity, InventoryItem item, int targetPart, bool equip)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.DOEQUIP);
            packet.Write(item.UniqueId);
            packet.Write((byte)0); // Guild id
            packet.Write(Convert.ToByte(equip));
            packet.Write(item.Id);
            packet.Write(item.Refines);
            packet.Write(0); // flag
            packet.Write(targetPart);

            SendToVisible(packet, entity, sendToPlayer: true);
        }

        /// <inheritdoc />
        public void SendItemCreation(IPlayerEntity entity, InventoryItem item)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.CREATEITEM);
            packet.Write<byte>(0);
            item.Serialize(packet);
            packet.Write<byte>(1);
            packet.Write((byte)item.UniqueId);
            packet.Write((short)item.Quantity);

            SendToPlayer(entity, packet);
        }

        /// <inheritdoc />
        public void SendItemUpdate(IPlayerEntity entity, UpdateItemType updateType, int uniqueId, int value)
        {
            using var packet = new FFPacket();
            
            packet.StartNewMergedPacket(entity.Id, SnapshotType.UPDATE_ITEM);
            packet.Write<byte>(0);
            packet.Write((byte)uniqueId);
            packet.Write((byte)updateType);
            packet.Write(value);
            packet.Write(0); // time

            SendToPlayer(entity, packet);
        }
    }
}
