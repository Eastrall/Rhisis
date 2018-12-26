using Ether.Network.Packets;
using System;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="OpenShopWindowPacket"/> packet structure.
    /// </summary>
    public struct OpenShopWindowPacket : IEquatable<OpenShopWindowPacket>
    {
        /// <summary>
        /// Gets the selected object id.
        /// </summary>
        public uint ObjectId { get; }

        /// <summary>
        /// Creates a new <see cref="OpenShopWindowPacket"/> instance.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public OpenShopWindowPacket(INetPacketStream packet)
        {
            this.ObjectId = packet.Read<uint>();
        }

        /// <summary>
        /// Compare two <see cref="OpenShopWindowPacket"/> instances.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(OpenShopWindowPacket other) => this.ObjectId == other.ObjectId;
    }
}
