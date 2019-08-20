using Rhisis.Core.Structures;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;

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
        IMonsterEntity CreateMonster(IMapInstance currentMap, IMapLayer currentMapLayer, int moverId, Vector3 position);
    }
}
