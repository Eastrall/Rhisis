using Rhisis.Game.Common;
using Rhisis.Protocol;

namespace Rhisis.Game.Protocol.Packets.World.Client;

public class MotionPacket
{
    /// <summary>
    /// Gets the motion.
    /// </summary>
    public ObjectMessageType MotionEnum { get; private set; }

    public MotionPacket(FFPacket packet)
    {
        MotionEnum = (ObjectMessageType)packet.ReadInt32();
    }
}
