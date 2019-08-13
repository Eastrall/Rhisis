using Rhisis.Core.Common;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Interaction
{
    public interface IInteractionSystem
    {
        /// <summary>
        /// Sets a target.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="targetObjectId">Target object id.</param>
        /// <param name="targetMode">Target mode.</param>
        void SetTarget(IPlayerEntity player, uint targetObjectId, TargetModeType targetMode);
    }
}