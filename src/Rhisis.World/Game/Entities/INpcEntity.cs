using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Components;

namespace Rhisis.World.Game.Entities
{
    public interface INpcEntity : ILivingEntity, IWorldEntity
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
    }
}
