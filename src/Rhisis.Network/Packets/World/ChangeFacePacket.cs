using System;
using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="ChangeFacePacket"/> structure.
    /// </summary>
    public class ChangeFacePacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the object id.
        /// </summary>
        public uint ObjectId { get; private set; }

        /// <summary>
        /// Gets the face number.
        /// </summary>
        public uint FaceNumber { get; private set; }

        /// <summary>
        /// Gets the cost.
        /// </summary>
        public int Cost { get; private set; }

        /// <summary>
        /// Gets if a coupon will be used.
        /// </summary>
        public bool UseCoupon { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.ObjectId = packet.Read<uint>();
            this.FaceNumber = packet.Read<uint>();
            this.Cost = packet.Read<int>();
            this.UseCoupon = Convert.ToBoolean(packet.Read<int>());
        }
    }
}