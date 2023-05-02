using Rhisis.Game.Common;
using Rhisis.Protocol;

namespace Rhisis.Game.Protocol.Packets.World.Client;

public class MotionPacket
{
    /// <summary>
    /// Gets the motion.
    /// </summary>
    public ObjectMessageType Motion { get; private set; }

    public MotionPacket(FFPacket packet)
    {
        Motion = (ObjectMessageType)packet.ReadInt32();
    }
}
