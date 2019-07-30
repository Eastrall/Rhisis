namespace Rhisis.World.Game.Maps
{
    /// <summary>
    /// Provides a mechanism to manage all maps of a world server.
    /// </summary>
    public interface IMapManager
    {
        /// <summary>
        /// Gets a map instance by its id.
        /// </summary>
        /// <param name="id">Map id.</param>
        /// <returns>Map instance.</returns>
        IMapInstance GetMap(int id);

        /// <summary>
        /// Loads all maps.
        /// </summary>
        void Load();
    }
}
