using Microsoft.Extensions.Logging;
using Rhisis.Game.Entities;
using Rhisis.Game.Protocol.Packets.World.Client;
using Rhisis.Protocol;
using Rhisis.Protocol.Handlers;

namespace Rhisis.WorldServer.Handlers;

[PacketHandler(PacketType.MOTION)]
internal sealed class MotionHandler : WorldPacketHandler
{
    /// <summary>
    /// Creates a new <see cref="MotionHandler"/> instance.
    /// </summary>
    public MotionHandler() { }

    public void Execute(MotionPacket packet)
    {
        Player.Motion(packet.MotionEnum);
    }
}

