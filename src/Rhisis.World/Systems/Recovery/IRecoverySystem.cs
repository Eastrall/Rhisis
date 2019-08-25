using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Recovery
{
    /// <summary>
    /// Game system that manages all recoveries. HP, MP, FP, etc...
    /// </summary>
    public interface IRecoverySystem
    {
        /// <summary>
        /// Process the idle heal.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="isSitted">Player sitting state.</param>
        void IdleRecevory(IPlayerEntity player, bool isSitted = false);
    }
}
