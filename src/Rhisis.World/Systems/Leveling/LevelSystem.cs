using Microsoft.Extensions.Logging;
using Rhisis.Core.Common.Formulas;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Leveling.EventArgs;

namespace Rhisis.World.Systems.Leveling
{
    /// <summary>
    /// Leveling system.
    /// </summary>
    [System(SystemType.Notifiable)]
    public sealed class LevelSystem : ISystem
    {
        private readonly ILogger<LevelSystem> _logger;
        private readonly IGameResources _gameResources;

        public LevelSystem(ILogger<LevelSystem> logger, IGameResources gameResources)
        {
            this._logger = logger;
            this._gameResources = gameResources;
        }

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (!(entity is IPlayerEntity player))
            {
                this._logger.LogError($"Cannot execute Level System. {entity.Object.Name} is not a player.");
                return;
            }

            if (!args.GetCheckArguments())
            {
                this._logger.LogError($"Cannot execute Level System action: {args.GetType()} due to invalid arguments.");
                return;
            }

            switch (args)
            {
                case ExperienceEventArgs e:
                    this.GiveExperience(player, e);
                    break;
                default:
                    this._logger.LogWarning("Unknown level system action type: {0} for player {1}", args.GetType(), entity.Object.Name);
                    break;
            }
        }

        /// <summary>
        /// Give experience to a player.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="e">Experience event info.</param>
        private void GiveExperience(IPlayerEntity player, ExperienceEventArgs e)
        {
            // Eastrall: Quick fix for not going beyond level 15. Will be removed with job system.
            if (player.Object.Level >= 15)
                return;

            long experience = this.CalculateExtraExperience(player, e.Experience);

            // TODO: experience to party

            if (this.GiveExperienceToPlayer(player, experience))
            {
                WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.HP, player.Health.Hp);
                WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.MP, player.Health.Mp);
                WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.FP, player.Health.Fp);
                WorldPacketFactory.SendPlayerSetLevel(player, player.Object.Level);
                WorldPacketFactory.SendPlayerStatsPoints(player);
            }

            WorldPacketFactory.SendPlayerExperience(player);
            // TODO: send packet to friends, messenger, guild, couple, party, etc...
        }

        /// <summary>
        /// Give experience to a player and returns a boolean value that indicates if the player has level up.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="experience">Experience to give.</param>
        /// <returns>True if the player has level up; false otherwise.</returns>
        private bool GiveExperienceToPlayer(IPlayerEntity player, long experience)
        {
            int nextLevel = player.Object.Level + 1;
            CharacterExpTableData nextLevelExpTable = this._gameResources.ExpTables.GetCharacterExp(nextLevel);
            player.PlayerData.Experience += experience;

            if (player.PlayerData.Experience >= nextLevelExpTable.Exp) // Level up
            {
                long remainingExp = player.PlayerData.Experience - nextLevelExpTable.Exp;

                this.ProcessLevelUp(player, (ushort)nextLevelExpTable.Gp);
                
                if (remainingExp > 0)
                    this.GiveExperienceToPlayer(player, remainingExp); // Multiple level up

                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates extra experience with scrolls, events, skill bonus, etc...
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="experience">Current experience.</param>
        /// <returns>Base experience with extra experience.</returns>
        private long CalculateExtraExperience(IPlayerEntity player, long experience)
        {
            // TODO: add exp scrolls logic here

            return experience;
        }

        /// <summary>
        /// Process the level up logic and reward attribution.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="statPoints">Statistics points.</param>
        private void ProcessLevelUp(IPlayerEntity player, ushort statPoints)
        {
            player.Object.Level += 1;

            if (player.Object.Level != player.PlayerData.DeathLevel)
            {
                player.Statistics.SkillPoints += (ushort)(((player.Object.Level - 1) / 20) + 2);
                player.Statistics.StatPoints += statPoints;
            }

            int strength = player.Attributes[DefineAttributes.STR];
            int stamina = player.Attributes[DefineAttributes.STA];
            int dexterity = player.Attributes[DefineAttributes.DEX];
            int intelligence = player.Attributes[DefineAttributes.INT];

            player.PlayerData.Experience = 0;
            player.Health.Hp = HealthFormulas.GetMaxOriginHp(player.Object.Level, stamina, player.PlayerData.JobData.MaxHpFactor);
            player.Health.Mp = HealthFormulas.GetMaxOriginMp(player.Object.Level, intelligence, player.PlayerData.JobData.MaxMpFactor, true);
            player.Health.Fp = HealthFormulas.GetMaxOriginFp(player.Object.Level, stamina, dexterity, strength, player.PlayerData.JobData.MaxFpFactor, true);
        }
    }
}
