﻿using LiteNetwork.Protocol.Abstractions;
using Rhisis.Core.Structures;
using Rhisis.Game.Abstractions.Protocol;

namespace Rhisis.Network.Packets.World
{
    public class DropItemPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the item type
        /// </summary>
        public uint ItemType { get; private set; }

        /// <summary>
        /// Gets the unique item id.
        /// </summary>
        public int ItemUniqueId { get; private set; }

        /// <summary>
        /// Gets the item quantity.
        /// </summary>
        public int ItemQuantity { get; private set; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <inhertidoc />
        public void Deserialize(ILitePacketStream packet)
        {
            ItemType = packet.Read<uint>();
            ItemUniqueId = packet.Read<int>();
            ItemQuantity = packet.Read<short>();
            Position = new Vector3(packet.Read<float>(), packet.Read<float>(), packet.Read<float>());
        }
    }
}