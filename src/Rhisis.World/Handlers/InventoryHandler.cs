using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Systems.Inventory;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.World.Handlers
{
    /// <summary>
    /// Handles all inventory packets.
    /// </summary>
    [Handler]
    public class InventoryHandler
    {
        private readonly IInventorySystem _inventorySystem;

        /// <summary>
        /// Creates a new <see cref="InventoryHandler"/> instance.
        /// </summary>
        /// <param name="inventorySystem">Inventory System.</param>
        public InventoryHandler(IInventorySystem inventorySystem)
        {
            this._inventorySystem = inventorySystem;
        }

        /// <summary>
        /// Handles the move item request.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        [HandlerAction(PacketType.MOVEITEM)]
        public void OnMoveItem(IWorldClient client, MoveItemPacket packet)
        {
            this._inventorySystem.MoveItem(client.Player, packet.SourceSlot, packet.DestinationSlot);
        }

        /// <summary>
        /// Handles the equip/unequip request.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        [HandlerAction(PacketType.DOEQUIP)]
        public void OnDoEquip(IWorldClient client, EquipItemPacket packet)
        {
            this._inventorySystem.EquipItem(client.Player, packet.UniqueId, packet.Part);
        }

        /// <summary>
        /// Handles the drop item request.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        [HandlerAction(PacketType.DROPITEM)]
        public void OnDropItem(IWorldClient client, DropItemPacket packet)
        {
            this._inventorySystem.DropItem(client.Player, packet.ItemUniqueId, packet.ItemQuantity);
        }

        /// <summary>
        /// Handles the use item request.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        [HandlerAction(PacketType.DOUSEITEM)]
        public void OnUseItem(IWorldClient client, DoUseItemPacket packet)
        {
            this._inventorySystem.UseItem(client.Player, packet.UniqueItemId, packet.Part);
        }
    }
}
