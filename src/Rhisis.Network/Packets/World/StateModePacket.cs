using Ether.Network.Packets;
using Rhisis.Core.Data;

namespace Rhisis.Network.Packets.World
{
    public class StateModePacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the client state mode.
        /// </summary>
        public StateMode StateMode { get; private set; }

        /// <summary>
        /// Gets the client state mode flag.
        /// </summary>
        public StateModeBaseMotion Flag { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.StateMode = (StateMode)packet.Read<int>();
            this.Flag = (StateModeBaseMotion)packet.Read<byte>();
        }
    }
}
