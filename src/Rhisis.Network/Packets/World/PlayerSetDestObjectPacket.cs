using Ether.Network.Packets;
using System;

namespace Rhisis.Network.Packets.World
{
    public struct PlayerSetDestObjectPacket : IEquatable<PlayerSetDestObjectPacket>
    {
        /// <summary>
        /// Gets the targeted object id.
        /// </summary>
        public uint TargetId { get; }

        /// <summary>
        /// Gets the distance.
        /// </summary>
        public float Distance { get; }

        /// <summary>
        /// Creates a new <see cref="PlayerSetDestObjectPacket"/> instance.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public PlayerSetDestObjectPacket(INetPacketStream packet)
        {
            this.TargetId = packet.Read<uint>();
            this.Distance = packet.Read<float>();
        }

        /// <summary>
        /// Compates two <see cref="PlayerSetDestObjectPacket"/> instances.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PlayerSetDestObjectPacket other) => this.TargetId == other.TargetId && this.Distance == other.Distance;
    }
}
