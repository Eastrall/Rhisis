using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World
{
    public class ModifyStatusPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the strength.
        /// </summary>
        public ushort Strenght { get; private set; }

        /// <summary>
        /// Gets the stamina.
        /// </summary>
        public ushort Stamina { get; private set; }

        /// <summary>
        /// Gets the dexterity.
        /// </summary>
        public ushort Dexterity { get; private set; }

        /// <summary>
        /// Gets the intelligence.
        /// </summary>
        public ushort Intelligence { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.Strenght = (ushort)packet.Read<int>();
            this.Stamina = (ushort)packet.Read<int>();
            this.Dexterity = (ushort)packet.Read<int>();
            this.Intelligence = (ushort)packet.Read<int>();
        }
    }
}
