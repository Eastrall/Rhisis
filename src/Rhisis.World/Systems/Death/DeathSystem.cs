using Microsoft.Extensions.Logging;
using Rhisis.Core.Common.Formulas;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Teleport;

namespace Rhisis.World.Systems.Death
{
    [System(SystemType.Notifiable)]
    public sealed class DeathSystem : ISystem
    {
        // TODO: injection
        private readonly IGameResources _gameResources;
        private readonly IMapManager _mapManager;

        private readonly ILogger<DeathSystem> _logger = DependencyContainer.Instance.Resolve<ILogger<DeathSystem>>();
        private readonly WorldConfiguration _worldConfiguration = DependencyContainer.Instance.Resolve<WorldConfiguration>();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (!(entity is IPlayerEntity player))
            {
                this._logger.LogError($"Cannot execute DeathSystem. {entity.Object.Name} is not a player.");
                return;
            }

            IMapRevivalRegion revivalRegion = player.Object.CurrentMap.GetNearRevivalRegion(player.Object.Position);

            if (revivalRegion == null)
            {
                this._logger.LogError($"Cannot find any revival region for map '{player.Object.CurrentMap.Name}'.");
                return;
            }

            decimal recoveryRate = this._gameResources.Penalities.GetRevivalPenality(player.Object.Level) / 100;
            var jobData = player.PlayerData.JobData;

            int strength = player.Attributes[DefineAttributes.STR];
            int stamina = player.Attributes[DefineAttributes.STA];
            int dexterity = player.Attributes[DefineAttributes.DEX];
            int intelligence = player.Attributes[DefineAttributes.INT];

            player.Health.Hp = (int)(HealthFormulas.GetMaxOriginHp(player.Object.Level, stamina, jobData.MaxHpFactor) * recoveryRate);
            player.Health.Mp = (int)(HealthFormulas.GetMaxOriginMp(player.Object.Level, intelligence, jobData.MaxMpFactor, true) * recoveryRate);
            player.Health.Fp = (int)(HealthFormulas.GetMaxOriginFp(player.Object.Level, stamina, dexterity, strength, jobData.MaxFpFactor, true) * recoveryRate);

            if (player.Object.MapId != revivalRegion.MapId)
            {
                IMapInstance revivalMap = this._mapManager.GetMap(revivalRegion.MapId);

                if (revivalMap == null)
                {
                    this._logger.LogError($"Cannot find revival map with id '{revivalRegion.MapId}'.");
                    player.Connection.Server.DisconnectClient(player.Connection.Id);
                    return;
                }

                revivalRegion = revivalMap.GetRevivalRegion(revivalRegion.Key);
            }

            var teleportEvent = new TeleportEventArgs(revivalRegion.MapId, revivalRegion.RevivalPosition.X, revivalRegion.RevivalPosition.Z);
            player.NotifySystem<TeleportSystem>(teleportEvent);

            WorldPacketFactory.SendMotion(player, ObjectMessageType.OBJMSG_ACC_STOP | ObjectMessageType.OBJMSG_STOP_TURN | ObjectMessageType.OBJMSG_STAND);
            WorldPacketFactory.SendPlayerRevival(player);
            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.HP, player.Health.Hp);
            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.MP, player.Health.Mp);
            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.FP, player.Health.Fp);

            this.ProcessDeathPenality(player);
        }

        /// <summary>
        /// Applies death penality if enabled.
        /// </summary>
        /// <param name="player">Death player entity.</param>
        private void ProcessDeathPenality(IPlayerEntity player)
        {
            if (this._worldConfiguration.Death.DeathPenalityEnabled)
            {
                decimal expLossPercent = this._gameResources.Penalities.GetDecExpPenality(player.Object.Level);

                if (expLossPercent <= 0)
                    return;

                player.PlayerData.Experience -= player.PlayerData.Experience * (long)(expLossPercent / 100m);
                player.PlayerData.DeathLevel = player.Object.Level;

                if (player.PlayerData.Experience < 0)
                {
                    if (this._gameResources.Penalities.GetLevelDownPenality(player.Object.Level))
                    {
                        CharacterExpTableData previousLevelExp = this._gameResources.ExpTables.GetCharacterExp(player.Object.Level - 1);

                        player.Object.Level--;
                        player.PlayerData.Experience = previousLevelExp.Exp + player.PlayerData.Experience;
                    }
                    else
                    {
                        player.PlayerData.Experience = 0;
                    }
                }

                WorldPacketFactory.SendPlayerExperience(player);
            }
        }
    }
}
