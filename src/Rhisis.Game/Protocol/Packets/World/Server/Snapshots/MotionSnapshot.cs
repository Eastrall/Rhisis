﻿using Rhisis.Game.Common;
using Rhisis.Game.Entities;
using Rhisis.Protocol;

namespace Rhisis.Game.Protocol.Packets.World.Server.Snapshots;

public class MotionSnapshot : FFSnapshot
{
    public MotionSnapshot(Mover mover, ObjectMessageType objectMessageType)
        : base(SnapshotType.MOTION, mover.ObjectId)
    {
        WriteInt32((int)objectMessageType);
    }
}
