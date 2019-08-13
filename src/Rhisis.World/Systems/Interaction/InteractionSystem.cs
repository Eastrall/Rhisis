using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Interaction
{
    [Injectable]
    public class InteractionSystem : IInteractionSystem
    {
        private readonly ILogger<InteractionSystem> _logger;

        /// <summary>
        /// Creates a new <see cref="InteractionSystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public InteractionSystem(ILogger<InteractionSystem> logger)
        {
            this._logger = logger;
        }

        /// <inheritdoc />
        public void SetTarget(IPlayerEntity player, uint targetObjectId, TargetModeType targetMode)
        {
            var targetEntity = player.FindEntity<IWorldEntity>(targetObjectId);

            if (targetEntity == null)
                return;

            if (targetMode == TargetModeType.Selected)
            {
                player.Interaction.TargetEntity = targetEntity;
                this._logger.LogDebug("Player {0} selected {1} as target.", player.Object.Name, targetEntity.Object.Name);
            }
            else if (targetMode == TargetModeType.Unselected)
            {
                player.Interaction.TargetEntity = null;
                this._logger.LogDebug("Player {0} cleared selection on {1}.", player.Object.Name, targetEntity.Object.Name);
            }
            else
            {
                this._logger.LogWarning("Player {0} raised an invalid or unknown target mode on {1}.", player.Object.Name, targetEntity.Object.Name);
            }
        }
    }
}