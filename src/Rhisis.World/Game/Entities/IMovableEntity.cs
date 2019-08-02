using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;

namespace Rhisis.World.Game.Entities
{
    /// <summary>
    /// Defines an entity that can move on the world.
    /// </summary>
    public interface IMovableEntity : IWorldEntity
    {
        /// <summary>
        /// Gets or sets the moving component of the entity.
        /// </summary>
        MovableComponent Moves { get; set; }

        /// <summary>
        /// Gets or sets the follow component of the entity.
        /// </summary>
        FollowComponent Follow { get; set; }

        /// <summary>
        /// Gets the timer component.
        /// </summary>
        TimerComponent Timers { get; set; }
    }
}
