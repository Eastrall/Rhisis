using Ether.Network.Packets;
using Rhisis.Core.Structures;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="PlayerMovedPacket"/> structure.
    /// </summary>
    public class PlayerMovedPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the BeginPosition.
        /// </summary>
        public Vector3 BeginPosition { get; private set; }

        /// <summary>
        /// Gets the DestinationPosition.
        /// </summary>
        public Vector3 DestinationPosition { get; private set; }

        /// <summary>
        /// Gets the Angle.
        /// </summary>
        public float Angle { get; private set; }

        /// <summary>
        /// Gets the object state (MovingFlag).
        /// </summary>
        public uint State { get; private set; }

        /// <summary>
        /// Gets the state flag. (Motion flag)
        /// </summary>
        public uint StateFlag { get; private set; }

        /// <summary>
        /// Gets the motion.
        /// </summary>
        public uint Motion { get; private set; }

        /// <summary>
        /// Gets the motion ex.
        /// </summary>
        public int MotionEx { get; private set; }

        /// <summary>
        /// Gets the loop.
        /// </summary>
        public int Loop { get; private set; }

        /// <summary>
        /// Gets the motion option.
        /// </summary>
        public uint MotionOption { get; private set; }

        /// <summary>
        /// Gets the tick count.
        /// </summary>
        public long TickCount { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.BeginPosition = new Vector3(packet.Read<float>(), packet.Read<float>(), packet.Read<float>());
            this.DestinationPosition = new Vector3(packet.Read<float>(), packet.Read<float>(), packet.Read<float>());
            this.Angle = packet.Read<float>();
            this.State = packet.Read<uint>();
            this.StateFlag = packet.Read<uint>();
            this.Motion = packet.Read<uint>();
            this.MotionEx = packet.Read<int>();
            this.Loop = packet.Read<int>();
            this.MotionOption = packet.Read<uint>();
            this.TickCount = packet.Read<long>();
        }
    }
}