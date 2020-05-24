using Rhisis.Core.Data;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Systems.SpecialEffect
{
    public interface ISpecialEffectSystem
    {
        /// <summary>
        /// Starts a new special effect.
        /// </summary>
        /// <param name="entity">Entity that activates the special effect.</param>
        /// <param name="specialEffect">Special effect to start.</param>
        void StartSpecialEffect(IWorldEntity entity, DefineSpecialEffects specialEffect, bool noFollowSfx);

        /// <summary>
        /// Sets the player's state mode base motion.
        /// </summary>
        /// <param name="entity">Entity that activates or deactivates the state mode.</param>
        /// <param name="motionState"></param>
        /// <param name="item"></param>
        void SetStateModeBaseMotion(IWorldEntity entity, StateModeBaseMotion motionState, InventoryItem item = null);
    }
}
