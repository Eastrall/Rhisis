using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Inventory.EventArgs;

namespace Rhisis.World.Handlers
{
    public static class InventoryHandler
    {
        [PacketHandler(PacketType.MOVEITEM)]
        public static void OnMoveItem(WorldClient client, INetPacketStream packet)
        {
            var itemType = packet.Read<byte>();
            var sourceSlot = packet.Read<byte>();
            var destinationSlot = packet.Read<byte>();
            var inventoryEvent = new InventoryMoveEventArgs(sourceSlot, destinationSlot);

            client.Player.NotifySystem<InventorySystem>(inventoryEvent);
        }

        [PacketHandler(PacketType.DOEQUIP)]
        public static void OnDoEquip(WorldClient client, INetPacketStream packet)
        {
            var uniqueId = packet.Read<int>();
            var part = packet.Read<int>();
            var inventoryEvent = new InventoryEquipEventArgs(uniqueId, part);

            client.Player.NotifySystem<InventorySystem>(inventoryEvent);
        }
    }
}
