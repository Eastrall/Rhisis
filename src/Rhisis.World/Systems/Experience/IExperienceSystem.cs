using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Leveling
{
    public interface IExperienceSystem
    {
        /// <summary>
        /// Give experience to a player.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="experience">Experience to give.</param>
        void GiveExeperience(IPlayerEntity player, long experience);
    }
}
