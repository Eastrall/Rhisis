using System;
using Sylver.Network.Data;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="RemoveInventoryItemPacket"/> structure.
    /// </summary>
    public class RemoveInventoryItemPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the unique item id.
        /// </summary>
        public int ItemUniqueId { get; private set; }

        /// <summary>
        /// Gets the item quantity.
        /// </summary>
        public int ItemQuantity { get; private set; }

        /// <summary>
        /// Creates a new <see cref="RemoveInventoryItemPacket"/> object.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public void Deserialize(INetPacketStream packet)
        {
            this.ItemUniqueId = packet.Read<int>();
            this.ItemQuantity = packet.Read<int>();
        }

        /// <summary>
        /// Compares two <see cref="RemoveInventoryItemPacket"/>.
        /// </summary>
        /// <param name="other">Other <see cref="RemoveInventoryItemPacket"/></param>
        public bool Equals(RemoveInventoryItemPacket other)
        {
            return this.ItemUniqueId == other.ItemUniqueId && this.ItemQuantity == other.ItemQuantity;
        }
    }
}