using Ether.Network.Packets;
using System.IO;
using System.Text;

namespace Rhisis.Network
{
    /// <summary>
    /// Represents a FlyFF packet.
    /// </summary>
    public class FFPacket : NetPacketStream
    {
        public const byte Header = 0x5E;
        public const uint NullId = 0xFFFFFFFF;
        
        private short _mergedPacketCount;

        /// <summary>
        /// Gets the FFPacket buffer.
        /// </summary>
        public override byte[] Buffer => this.BuildPacketBuffer();

        /// <summary>
        /// Creates a new FFPacket in write-only mode.
        /// </summary>
        public FFPacket()
        {
            base.Write(Header);
            base.Write(0);
        }

        /// <summary>
        /// Creates a new FFPacket with a header.
        /// </summary>
        /// <param name="packetHeader"></param>
        public FFPacket(object packetHeader)
            : this()
        {
            this.WriteHeader(packetHeader);
        }

        /// <summary>
        /// Creates a new FFPacket in read-only mode.
        /// </summary>
        /// <param name="buffer"></param>
        public FFPacket(byte[] buffer)
            : base(buffer)
        {
        }

        /// <summary>
        /// Write packet header.
        /// </summary>
        /// <param name="packetHeader">FFPacket header</param>
        public void WriteHeader(object packetHeader) => this.Write((uint)packetHeader);

        /// <summary>
        /// Write data into a packet.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public override void Write<T>(T value)
        {
            if (typeof(T) == typeof(string))
            {
                this.WriteString(value as string);
                return;
            }

            base.Write(value);
        }

        /// <summary>
        /// Read data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T Read<T>()
        {
            if (typeof(T) == typeof(string))
                return (T)(object)this.ReadString();

            return base.Read<T>();
        }

        /// <summary>
        /// Read FF String.
        /// </summary>
        /// <returns></returns>
        private string ReadString()
        {
            var size = this.Read<int>();

            return size == 0 ? string.Empty : Encoding.GetEncoding(1252).GetString(this.ReadBytes(size));
        }

        /// <summary>
        /// Read a specified number of bytes.
        /// </summary>
        /// <param name="count">Number of bytes to read</param>
        /// <returns></returns>
        public byte[] ReadBytes(int count) => this.ReadArray<byte>(count);

        /// <summary>
        /// Write FF string.
        /// </summary>
        /// <param name="value"></param>
        private void WriteString(string value)
        {
            if (value.Length > 0)
                base.Write(Encoding.GetEncoding(0).GetBytes(value));
        }

        /// <summary>
        /// Builds the packet buffer.
        /// </summary>
        /// <returns></returns>
        private byte[] BuildPacketBuffer()
        {
            long oldPointer = this.Position;

            this.Seek(1, SeekOrigin.Begin);
            base.Write(this.Size - 5);
            this.Seek(oldPointer, SeekOrigin.Begin);

            return base.Buffer;
        }

        /// <summary>
        /// Start a new merged packet.
        /// </summary>
        /// <param name="moverId"></param>
        /// <param name="command"></param>
        /// <param name="mainCommand"></param>
        public void StartNewMergedPacket(uint moverId, object command, uint mainCommand)
        {
            var packet = (uint)command;

            if (this._mergedPacketCount == 0)
            {
                this.Write((int)mainCommand);
                this.Write(0);
                this.Write(++this._mergedPacketCount);
            }
            else
            {
                this.Seek(13, SeekOrigin.Begin);
                this.Write(++this._mergedPacketCount);
                this.Seek(0, SeekOrigin.End);
            }

            this.Write(moverId);
            this.Write((short)packet);
        }

        /// <summary>
        /// Start a new merged packet.
        /// </summary>
        /// <param name="moverId"></param>
        /// <param name="command"></param>
        public void StartNewMergedPacket(uint moverId, object command) => this.StartNewMergedPacket(moverId, command, 0xFFFFFF00);
    }
}
