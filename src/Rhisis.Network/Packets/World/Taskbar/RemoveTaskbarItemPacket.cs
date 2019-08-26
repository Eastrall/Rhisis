using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World.Taskbar
{
    public class RemoveTaskbarItemPacket : RemoveTaskbarAppletPacket, IPacketDeserializer
    {
        /// <summary>
        ///  Gets the slot level index from the item taskbar to be removed.
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