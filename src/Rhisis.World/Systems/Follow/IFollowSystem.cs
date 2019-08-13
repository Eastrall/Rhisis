using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Follow
{
    public interface IFollowSystem
    {
        /// <summary>
        /// Follows a target entity and finds it by its id.
        /// </summary>
        /// <param name="livingEntity">Current living entity.</param>
        /// <param name="targetId">Target entity id to follow.</param>
        /// <param name="distance">Distance.</param>
        void Follow(ILivingEntity livingEntity, uint targetId, float distance = 1f);

        /// <summary>
        /// Follows a target entity.
        /// </summary>
        /// <param name="livingEntity">Current living entity.</param>
        /// <param name="targetEntity">Target entity to follow.</param>
        /// <param name="distance">Distance.</param>
        void Follow(ILivingEntity livingEntity, ILivingEntity targetEntity, float distance = 1f);
    }
}
