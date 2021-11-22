﻿using Rhisis.Game.Abstractions.Protocol;
using LiteNetwork.Protocol.Abstractions;

namespace Rhisis.Network.Packets.World
{
    public class PlayerAnglePacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the Angle.
        /// </summary>
        public float Angle { get; private set; }

        /// <summary>
        /// Gets the X angle.
        /// </summary>
        public float AngleX { get; private set; }

        /// <inheritdoc />
        public void Deserialize(ILitePacketStream packet)
        {
            Angle = packet.Read<float>();
            AngleX = packet.Read<float>();
        }
    }
}