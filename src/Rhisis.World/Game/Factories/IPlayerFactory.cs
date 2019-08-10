using Rhisis.Database.Entities;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Game.Factories
{
    public interface IPlayerFactory
    {
        /// <summary>
        /// Creates a new player from a <see cref="DbCharacter"/> entity.
        /// </summary>
        /// <param name="character">Player's database representation.</param>
        /// <returns>New player entity.</returns>
        IPlayerEntity CreatePlayer(DbCharacter character);
    }
}
