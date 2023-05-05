using Microsoft.Extensions.Logging;
using Rhisis.Game.Entities;
using Rhisis.Game.Protocol.Packets.World.Client;
using Rhisis.Protocol;
using Rhisis.Protocol.Handlers;

namespace Rhisis.WorldServer.Handlers;

[PacketHandler(PacketType.MOTION)]
internal sealed class MotionHandler : WorldPacketHandler
{
    private readonly ILogger<MotionHandler> _logger;

    /// <summary>
    /// Creates a new <see cref="MotionHandler"/> instance.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public MotionHandler(ILogger<MotionHandler> logger)
    {
        _logger = logger;
    }

    public void Execute(MotionPacket packet)
    {
        Player.Motion(packet.MotionEnum);
    }
}

