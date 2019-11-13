using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.NpcBuff
{
    public interface INpcBuffSystem
    {
        void NpcBuff(IPlayerEntity player, string buff);
    }
}
