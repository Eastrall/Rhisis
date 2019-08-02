using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Systems.Interaction.EventArgs;

namespace Rhisis.World.Systems.Interaction
{
    [System(SystemType.Notifiable)]
    public class InteractionSystem : ISystem
    {
        private static readonly ILogger Logger = DependencyContainer.Instance.Resolve<ILogger<InteractionSystem>>();

        /// <summary>
        /// Defines a const used to select a target.
        /// </summary>
        private const byte SELECT_TARGET = 2;

        /// <summary>
        /// Defines a const used to unselect a target.
        /// </summary>
        private const byte UNSELECT_TARGET = 1;

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <summary>
        /// Executes the <see cref="InteractionSystem"/> logic.
        /// </summary>
        /// <param name="entity">Current entity</param>
        public void Execute(IWorldEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity playerEntity))
                return;

            if (!e.GetCheckArguments())
            {
                Logger.LogError("Cannot execute interaction action: {0} due to invalid arguments.", e.GetType());
                return;
            }

            switch(e)
            {
                case SetTargetEventArgs setTargetEventArgs:
                    this.SetTarget(playerEntity, setTargetEventArgs);
                    break;
                default:
                    Logger.LogWarning("Unknown interaction action type: {0} for player {1}", e.GetType(), entity.Object.Name);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private void SetTarget(IPlayerEntity player, SetTargetEventArgs e)
        {
            var targetEntity = player.FindEntity<IWorldEntity>(e.TargetId);
            if (targetEntity == null)
                return;

            if (e.TargetingMode == SELECT_TARGET)
            {
                player.Interaction.TargetEntity = targetEntity;
                Logger.LogDebug("Player {0} selected {1} as target.", player.Object.Name, targetEntity.Object.Name);
            }
            else if(e.TargetingMode == UNSELECT_TARGET)
            {
                player.Interaction.TargetEntity = null;
                Logger.LogDebug("Player {0} cleared selection on {1}.", player.Object.Name, targetEntity.Object.Name);
            }
            else
            {
                Logger.LogWarning("Player {0} raised an invalid or unknown target mode on {1}.", player.Object.Name, targetEntity.Object.Name);
            }
        }
    }
}