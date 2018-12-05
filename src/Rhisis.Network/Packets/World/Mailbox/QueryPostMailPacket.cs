using System;
using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World.Mailbox
{
    public struct QueryPostMailPacket : IEquatable<QueryPostMailPacket>
    {
        /// <summary>
        /// Gets the slot of the item.
        /// </summary>
        public byte ItemSlot { get; }

        /// <summary>
        /// Gets the quantity of the item.
        /// </summary>
        public short ItemQuantity { get; }

        /// <summary>
        /// Gets the receiver's name.
        /// </summary>
        public string Receiver { get; }

        /// <summary>
        /// Gets the amount of gold.
        /// </summary>
        public int Gold { get; }

        /// <summary>
        /// Gets the title of the mail.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the text of the mail.
        /// </summary>
        public string Text { get; }


        /// <summary>
        /// Creates a new <see cref="QueryPostMailPacket"/> instance.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public QueryPostMailPacket(INetPacketStream packet)
        {
            this.ItemSlot = packet.Read<byte>();
            this.ItemQuantity = packet.Read<short>();
            this.Receiver = packet.Read<string>();
            this.Gold = packet.Read<int>();
            this.Title = packet.Read<string>();
            this.Text = packet.Read<string>();
        }

        /// <summary>
        /// Compares two <see cref="QueryPostMailPacket"/> objects.
        /// </summary>
        /// <param name="other">Other <see cref="QueryPostMailPacket"/></param>
        /// <returns></returns>
        public bool Equals(QueryPostMailPacket other)
        {
            throw new NotImplementedException();
        }
    }
}
