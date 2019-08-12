using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Teleport
{
    public interface ITeleportSystem
    {
        void Teleport(IPlayerEntity player, int mapId, float x, float? y, float z, float angle = 0);
    }
}
