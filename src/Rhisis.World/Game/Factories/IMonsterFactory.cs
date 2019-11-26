using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Game.Maps.Regions;

namespace Rhisis.World.Game.Factories
{
    public interface IMonsterFactory
    {
        /// <summary>
        /// Creates a new monster entity in the given context.
        /// </summary>
        /// <param name="currentMap">Current map instance.</param>
        /// <param name="currentMapLayer">Current map layer.</param>
        /// <param name="moverId">Monster mover id.</param>
        /// <param name="region">Monster region.</param>
        /// <param name="respawn">Monster respawn ability.</param>
        /// <returns>New monster.</returns>
        IMonsterEntity CreateMonster(IMapInstance currentMap, IMapLayer currentMapLayer, int moverId, IMapRespawnRegion region, bool respawn = false);
    }
}
