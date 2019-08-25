using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems
{
    /// <summary>
    /// Provides a mechanism that respawns monster or items.
    /// </summary>
    public interface IRespawnSystem
    {
        /// <summary>
        /// Respawns monsters or items after an elapsed amount of time.
        /// </summary>
        /// <param name="entity">Entity to respawn.</param>
        void Execute(IWorldEntity entity);
    }
}
