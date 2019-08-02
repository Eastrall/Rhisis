using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Maps.Regions;

namespace Rhisis.World.Game.Entities
{
    public interface IMonsterEntity : IWorldEntity, IMovableEntity, ILivingEntity
    {
        /// <summary>
        /// Gets or sets the parent region of the monster.
        /// </summary>
        IMapRespawnRegion Region { get; set; }

        /// <summary>
        /// Gets the monster's behavior.
        /// </summary>
        IBehavior Behavior { get; set; }

        /// <summary>
        /// Gets or sets the monster's mover data.
        /// </summary>
        MoverData Data { get; set; }
    }
}
