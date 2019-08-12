using Ether.Network.Packets;
using System;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="SetHairPacket"/> structure.
    /// </summary>
    public class SetHairPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the hair id.
        /// </summary>
        public byte HairId { get; private set; }

        /// <summary>
        /// Gets the red color.
        /// </summary>
        public byte R { get; private set; }

        /// <summary>
        /// Gets the green color.
        /// </summary>
        public byte G { get; private set; }

        /// <summary>
        /// Gets the blue color.
        /// </summary>
        public byte B { get; private set; }

        /// <summary>
        /// Gets if a coupon will be used.
        /// </summary>
        public bool UseCoupon { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.HairId = packet.Read<byte>();
            this.R = packet.Read<byte>();
            this.G = packet.Read<byte>();
            this.B = packet.Read<byte>();
            this.UseCoupon = Convert.ToBoolean(packet.Read<int>());
        }
    }
}
