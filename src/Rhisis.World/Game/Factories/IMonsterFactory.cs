using Rhisis.Core.Structures;
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
        /// <param name="position">Monster position.</param>
        /// <returns>New monster.</returns>
        IMonsterEntity CreateMonster(IMapInstance currentMap, IMapLayer currentMapLayer, int moverId, IMapRespawnRegion region);

        /// <summary>
        /// Creates a new monster entity in the given context duplicated from a given monster entity.
        /// </summary>
        /// <param name="monster">base monster.</param>
        /// <param name="position">position for the new monster.</param>
        /// <param name="respawn">is the mob able to respawn.</param>
        IMonsterEntity DuplicateMonster(IMonsterEntity monster, Vector3 position, bool respawn = false);
    }
}
