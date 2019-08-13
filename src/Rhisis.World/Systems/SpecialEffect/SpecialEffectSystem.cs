using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;

namespace Rhisis.World.Systems.SpecialEffect
{
    [Injectable]
    public sealed class SpecialEffectSystem : ISpecialEffectSystem
    {
        private readonly ISpecialEffectPacketFactory _specialEffectPacketFactory;
        private readonly IMoverPacketFactory _moverPacketFactory;

        /// <summary>
        /// Creates a new <see cref="SpecialEffectSystem"/> instance.
        /// </summary>
        /// <param name="specialEffectPacketFactory"></param>
        public SpecialEffectSystem(ISpecialEffectPacketFactory specialEffectPacketFactory, IMoverPacketFactory moverPacketFactory)
        {
            this._specialEffectPacketFactory = specialEffectPacketFactory;
            this._moverPacketFactory = moverPacketFactory;
        }

        /// <inheritdoc />
        public void SetStateModeBaseMotion(IWorldEntity entity, StateModeBaseMotion motionState, Item item = null)
        {
            if (motionState == StateModeBaseMotion.BASEMOTION_ON)
            {
                entity.Object.StateMode |= StateMode.BASEMOTION_MODE;
            }
            else
            {
                entity.Object.StateMode &= ~StateMode.BASEMOTION_MODE;
            }

            this._moverPacketFactory.SendStateMode(entity, motionState, item);
        }

        /// <inheritdoc />
        public void StartSpecialEffect(IWorldEntity entity, DefineSpecialEffects specialEffect)
        {
            this._specialEffectPacketFactory.SendSpecialEffect(entity, specialEffect);
        }
    }
}
