using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;

namespace Rhisis.World.Systems.Visibility
{
    public interface IVisibilitySystem
    {
        /// <summary>
        /// Executes the visibility system.
        /// </summary>
        /// <param name="worldEntity">World entity.</param>
        void Execute(IWorldEntity worldEntity);

        /// <summary>
        /// Despawns an entity.
        /// </summary>
        /// <param name="worldEntity">Entity to despawn.</param>
        void DespawnEntity(IWorldEntity worldEntity);
    }
}
