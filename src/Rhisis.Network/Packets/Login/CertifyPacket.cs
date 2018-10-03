using Ether.Network.Packets;
using System;
using System.Linq;
using System.Text;

namespace Rhisis.Network.Packets.Login
{
    public struct CertifyPacket : IEquatable<CertifyPacket>
    {
        public string BuildVersion { get; }

        public string Username { get; }

        public string Password { get; }

        public byte[] EncryptedPassword { get; }

        public CertifyPacket(INetPacketStream packet, bool useEncryptedPassword)
        {
            this.BuildVersion = packet.Read<string>();
            this.Username = packet.Read<string>();
            this.Password = null;
            this.EncryptedPassword = null;

            if (useEncryptedPassword)
                this.EncryptedPassword = packet.ReadArray<byte>(16 * 42);
            else
                this.Password = packet.Read<string>();
        }

        public bool Equals(CertifyPacket other)
        {
            return this.BuildVersion == other.BuildVersion &&
                this.Username == other.Username &&
                this.Password == other.Password;
        }
    }
}