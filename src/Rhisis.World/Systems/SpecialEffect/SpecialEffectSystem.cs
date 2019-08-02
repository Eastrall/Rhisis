using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.SpecialEffect.EventArgs;

namespace Rhisis.World.Systems.SpecialEffect
{
    /// <summary>
    /// 
    /// </summary>
    [System(SystemType.Notifiable)]
    public sealed class SpecialEffectSystem : ISystem
    {
        private readonly ILogger<SpecialEffectSystem> _logger;

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Object;

        /// <summary>
        /// Creates a new <see cref="SpecialEffectSystem"/> instance.
        /// </summary>
        public SpecialEffectSystem()
        {
            this._logger = DependencyContainer.Instance.Resolve<ILogger<SpecialEffectSystem>>();
        }

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (entity == null)
            {
                this._logger.LogError($"Cannot execute {nameof(SpecialEffectSystem)}. Entity is null.");
                return;
            }

            if (!args.GetCheckArguments())
            {
                this._logger.LogError($"Cannot execute {nameof(SpecialEffectSystem)} action: {args.GetType()} due to invalid arguments.");
                return;
            }

            switch (args)
            {
                case SpecialEffectEventArgs e:
                    this.StartSpecialEffect(entity, e);
                    break;
                case SpecialEffectBaseMotionEventArgs e:
                    this.SetStateModeBaseMotion(entity, e);
                    break;
            }
        }

        /// <summary>
        /// Starts a new special effect.
        /// </summary>
        /// <param name="entity">Entity that activates the special effect.</param>
        /// <param name="e">Special effect event.</param>
        private void StartSpecialEffect(IWorldEntity entity, SpecialEffectEventArgs e)
        {
            WorldPacketFactory.SendSpecialEffect(entity, e.SpecialEffect);
        }

        /// <summary>
        /// Sets the player's state mode base motion.
        /// </summary>
        /// <param name="entity">Entity that activates or deactivates the state mode.</param>
        /// <param name="e">Special effect base motion event.</param>
        private void SetStateModeBaseMotion(IWorldEntity entity, SpecialEffectBaseMotionEventArgs e)
        {
            if (e.Motion == StateModeBaseMotion.BASEMOTION_ON)
            {
                entity.Object.StateMode |= StateMode.BASEMOTION_MODE;
            }
            else
            {
                entity.Object.StateMode &= ~StateMode.BASEMOTION_MODE;
            }

            WorldPacketFactory.SendStateMode(entity, e.Motion, e.Item);
        }
    }
}
