using System;
using Ether.Network.Packets;

namespace Rhisis.Core.Network.Packets.World
{
    public class SayPacket : IEquatable<SayPacket>
    {
        public int TargetSayId { get; }

        public SayPacket(INetPacketStream packet)
        {
            this.TargetSayId = packet.Read<int>();
        }

        public bool Equals(SayPacket other) => true;
    }

    public class SayPacketTwo : IEquatable<SayPacketTwo>
    {
        public string PrivateMessage { get; }


        public SayPacketTwo(INetPacketStream packet)
        {
            this.PrivateMessage = packet.Read<string>();
        }

        public bool Equals(SayPacketTwo other) => true;
    }
}
