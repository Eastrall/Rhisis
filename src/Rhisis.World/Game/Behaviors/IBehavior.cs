using Rhisis.World.Game.Core;

namespace Rhisis.World.Game.Behaviors
{
    /// <summary>
    /// Describes the behavior of an AI (Behavior)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBehavior<in T> where T : IEntity
    {
        /// <summary>
        /// Updates the AI/Behavior.
        /// </summary>
        /// <param name="entity">Entity to update</param>
        void Update(T entity);

        /// <summary>
        /// Fired when an entity arrives to its destination position.
        /// </summary>
        /// <param name="entity"></param>
        void OnArrived(T entity);
    }
}
