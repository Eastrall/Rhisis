using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.Core.Structures;

namespace Rhisis.World.Game.Entities
{
    public interface IMonsterEntity : IWorldEntity, IMovableEntity, ILivingEntity
    {
        /// <summary>
        /// Gets or sets the parent region of the monster.
        /// </summary>
        IMapRespawnRegion Region { get; set; }

        /// <summary>
        /// Gets or sets the monster's mover data.
        /// </summary>
        MoverData Data { get; set; }

        /// <summary>
        /// Gets or sets the an area of the spawned monster.
        /// </summary>
        Rectangle Rectangle { get; set; }
    }
}
