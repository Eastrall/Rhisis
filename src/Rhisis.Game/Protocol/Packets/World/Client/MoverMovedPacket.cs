using Rhisis.Game;
using Rhisis.Protocol;

namespace Rhisis.Game.Protocol.Packets.World.Client;

/// <summary>
/// Structure for the <see cref="PacketType.PLAYERMOVED"/> packet.
/// </summary>
public class MoverMovedPacket
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
    public int StateFlag { get; private set; }

    /// <summary>
    /// Gets the motion.
    /// </summary>
    public int Motion { get; private set; }

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
    public int MotionOption { get; private set; }

    /// <summary>
    /// Gets the tick count.
    /// </summary>
    public long TickCount { get; private set; }

    public MoverMovedPacket(FFPacket packet)
    {
        BeginPosition = new Vector3(packet.ReadSingle(), packet.ReadSingle(), packet.ReadSingle());
        DestinationPosition = new Vector3(packet.ReadSingle(), packet.ReadSingle(), packet.ReadSingle());
        Angle = packet.ReadSingle();
        State = packet.ReadUInt32();
        StateFlag = packet.ReadInt32();
        Motion = packet.ReadInt32();
        MotionEx = packet.ReadInt32();
        Loop = packet.ReadInt32();
        MotionOption = packet.ReadInt32();
        TickCount = packet.ReadInt64();
    }
}