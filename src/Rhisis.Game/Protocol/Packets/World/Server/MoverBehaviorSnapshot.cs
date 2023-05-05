using Rhisis.Game.Common;
using Rhisis.Game.Entities;
using Rhisis.Protocol;

namespace Rhisis.Game.Protocol.Packets.World.Server;

public sealed class MoverBehaviorSnapshot : FFSnapshot
{
    public MoverBehaviorSnapshot(Mover mover, Vector3 beginPosition, Vector3 destinationPosition, float angle, int state, int stateFlag, int motion, int motionEx, int loop, int motionOption, long tickCount)
        : base(SnapshotType.MOVERBEHAVIOR, mover.ObjectId)
    {
        WriteSingle(beginPosition.X);
        WriteSingle(beginPosition.Y);
        WriteSingle(beginPosition.Z);
        WriteSingle(destinationPosition.X);
        WriteSingle(destinationPosition.Y);
        WriteSingle(destinationPosition.Z);
        WriteSingle(angle);

        if (mover is Player player)
        {
            if (player.Mode.HasFlag(ModeType.TRANSPARENT_MODE))
            {
                WriteInt32((int)ObjectState.OBJSTA_STAND);
            }
            else
            {
                WriteInt32(state);
            }
        }
        else
        {
            WriteInt32(state);
        }

        WriteInt32(stateFlag);
        WriteInt32(motion);
        WriteInt32(motionEx);
        WriteInt32(loop);
        WriteInt32(motionOption);
        WriteInt64(tickCount);
    }
}