using Ether.Network.Server;
using Rhisis.World.Game.Entities;

namespace Rhisis.World
{
    public interface IWorldServer : INetServer
    {
        /// <summary>
        /// Gets a player entity by his id.
        /// </summary>
        /// <param name="id">Player id</param>
        /// <returns><see cref="IPlayerEntity"/></returns>
        IPlayerEntity GetPlayerEntity(uint id);

        /// <summary>
        /// Gets a player entity by his name
        /// </summary>
        /// <param name="name">Player name.</param>
        /// <returns><see cref="IPlayerEntity"/></returns>
        IPlayerEntity GetPlayerEntity(string name);

        /// <summary>
        /// Gets a player entity by its character id.
        /// </summary>
        /// <param name="id">Character id.</param>
        /// <returns><see cref="IPlayerEntity"/></returns>
        IPlayerEntity GetPlayerEntityByCharacterId(uint id);
    }
}
