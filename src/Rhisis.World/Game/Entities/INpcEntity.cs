using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;

namespace Rhisis.World.Game.Entities
{
    public interface INpcEntity : IEntity
    {
        /// <summary>
        /// Gets or sets the npc's shop item containers.
        /// </summary>
        /// <remarks>
        /// One item container represents one shop tab.
        /// </remarks>
        ItemContainerComponent[] Shop { get; set; }

        /// <summary>
        /// Gets the NPC data.
        /// </summary>
        NpcData Data { get; }

        /// <summary>
        /// Gets the NPC behavior.
        /// </summary>
        IBehavior Behavior { get; set; }

        /// <summary>
        /// Gets or sets the NPC timers.
        /// </summary>
        NpcTimerComponent Timers { get; set; }
    }
}
