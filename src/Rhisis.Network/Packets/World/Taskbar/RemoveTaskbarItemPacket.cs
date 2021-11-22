﻿using LiteNetwork.Protocol.Abstractions;

namespace Rhisis.Network.Packets.World.Taskbar
{
    public class RemoveTaskbarItemPacket : RemoveTaskbarAppletPacket
    {
        /// <summary>
        ///  Gets the slot level index from the item taskbar to be removed.
        /// </summary>
        public int SlotLevelIndex { get; private set; }

        /// <inheritdoc />
        public override void Deserialize(ILitePacketStream packet)
        {
            SlotLevelIndex = packet.Read<byte>();
            base.Deserialize(packet);
        }
    }
}