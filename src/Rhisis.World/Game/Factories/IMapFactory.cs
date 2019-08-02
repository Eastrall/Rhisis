using Rhisis.World.Game.Maps;

namespace Rhisis.World.Game.Factories
{
    /// <summary>
    /// Provides a mechanism to create maps.
    /// </summary>
    public interface IMapFactory
    {
        /// <summary>
        /// Creates a new <see cref="IMapInstance"/>.
        /// </summary>
        /// <returns>New map instance.</returns>
        IMapInstance Create(string mapPath, string mapName, int mapId);
    }
}
