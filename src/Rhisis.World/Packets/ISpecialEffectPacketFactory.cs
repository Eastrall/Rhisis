using Rhisis.Core.Data;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface ISpecialEffectPacketFactory
    {
        /// <summary>
        /// Sends a special effect to every entities around the given entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="specialEffectId">Special effect Id.</param>
        void SendSpecialEffect(IWorldEntity entity, int specialEffectId);

        /// <summary>
        /// Sends a special effect to every entities around the given entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="specialEffect">Special effect.</param>
        void SendSpecialEffect(IWorldEntity entity, DefineSpecialEffects specialEffect);
    }
}
