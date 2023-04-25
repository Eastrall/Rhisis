using Rhisis.Protocol;

namespace Rhisis.Game.Protocol.Packets.World.Client;

public class MotionPacket
{
    /// <summary>
    /// Gets the motion id.
    /// </summary>
    public int MotionId { get; private set; }

    public MotionPacket(FFPacket packet)
    {
        MotionId = packet.ReadInt32();
    }
}
