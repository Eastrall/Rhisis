using Rhisis.World.Game.Entities;

namespace Rhisis.World.Game.Behaviors
{
    /// <summary>
    /// Provides a mechanism to load and create behaviors.
    /// </summary>
    public interface IBehaviorManager
    {
        /// <summary>
        /// Gets the number of behaviors.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Loads all behaviors.
        /// </summary>
        void Load();

        /// <summary>
        /// Gets an entity custom behavior by its mover id.
        /// </summary>
        /// <param name="type">Behavior type.</param>
        /// <param name="entity">Entity that will receive the behavior.</param>
        /// <param name="moverId">Mover Id.</param>
        /// <returns>Entity behavior.</returns>
        IBehavior GetBehavior(BehaviorType type, IWorldEntity entity, int moverId);

        /// <summary>
        /// Gets the default behavior of an entity.
        /// </summary>
        /// <param name="type">Behavior type.</param>
        /// <param name="entity">Entity that will receive the default behavior.</param>
        /// <returns>Default entity behavior.</returns>
        IBehavior GetDefaultBehavior(BehaviorType type, IWorldEntity entity);
    }
}
