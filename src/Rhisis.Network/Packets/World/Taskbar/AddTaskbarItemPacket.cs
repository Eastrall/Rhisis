using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World.Taskbar
{
    public class AddTaskbarItemPacket : AddTaskbarAppletPacket, IPacketDeserializer
    {
        /// <summary>
        /// Gets the slot level index.
        /// </summary>
        public int SlotLevelIndex { get; private set; }

        /// <inheritdoc />
        public override void Deserialize(INetPacketStream packet)
        {
            this.SlotLevelIndex = packet.Read<byte>();
            base.Deserialize(packet);
        }
    }
}