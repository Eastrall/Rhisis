using Rhisis.Game.Common;
using Rhisis.Game.Entities;

namespace Rhisis.Protocol.Snapshots;

public class ModifyModeSnapshot : FFSnapshot
{
    public ModifyModeSnapshot(Player player, ModeType mode)
        : base(SnapshotType.MODIFYMODE, player.ObjectId)
    {
        WriteUInt32((uint)mode);
    }
}