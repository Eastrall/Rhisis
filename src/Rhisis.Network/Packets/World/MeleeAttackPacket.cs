using Ether.Network.Packets;
using Rhisis.Core.Data;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="MeleeAttackPacket"/> structure.
    /// </summary>
    public class MeleeAttackPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the attack message.
        /// </summary>
        public ObjectMessageType AttackMessage { get; private set; }

        /// <summary>
        /// Gets the object id.
        /// </summary>
        public uint ObjectId { get; private set; }
        
        /// <summary>
        /// Gets the unknown parameter.
        /// </summary>
        public int UnknownParameter { get; private set; }
        
        /// <summary>
        /// Gets the attack flags.
        /// </summary>
        public int AttackFlags { get; private set; }

        /// <summary>
        /// Gets the attack speed.
        /// </summary>
        public float WeaponAttackSpeed { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.AttackMessage = (ObjectMessageType)packet.Read<int>();
            this.ObjectId = packet.Read<uint>();
            this.UnknownParameter = packet.Read<int>(); // ??
            this.AttackFlags = packet.Read<int>() & 0xFFFF; // Attack flags ?!
            this.WeaponAttackSpeed = packet.Read<float>();
        }
    }
}
