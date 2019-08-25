using Microsoft.Extensions.Logging;
using Rhisis.Core.Common.Formulas;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.IO;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;

namespace Rhisis.World.Systems.Recovery
{
    /// <summary>
    /// Game system that manages all recoveries. HP, MP, FP, etc...
    /// </summary>
    [Injectable]
    public sealed class RecoverySystem : IRecoverySystem
    {
        /// <summary>
        /// Gets the number of seconds for next idle heal when the player is sitted.
        /// </summary>
        public const int NextIdleHealSit = 2;

        /// <summary>
        /// Gets the number of seconds for the idle heal when the player stands up.
        /// </summary>
        public const int NextIdleHealStand = 3;

        private readonly ILogger<RecoverySystem> _logger;

        /// <summary>
        /// Creates a new <see cref="RecoverySystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public RecoverySystem(ILogger<RecoverySystem> logger)
        {
            this._logger = logger;
        }

        /// <inheritdoc />
        public void IdleRecevory(IPlayerEntity player, bool isSitted = false)
        {
            int nextHealDelay = isSitted ? NextIdleHealSit : NextIdleHealStand;

            player.Timers.NextHealTime = Time.TimeInSeconds() + nextHealDelay;

            if (player.Health.IsDead)
                return;

            int strength = player.Attributes[DefineAttributes.STR];
            int stamina = player.Attributes[DefineAttributes.STA];
            int dexterity = player.Attributes[DefineAttributes.DEX];
            int intelligence = player.Attributes[DefineAttributes.INT];

            int maxHp = HealthFormulas.GetMaxOriginHp(player.Object.Level, stamina, player.PlayerData.JobData.MaxHpFactor);
            int maxMp = HealthFormulas.GetMaxOriginMp(player.Object.Level, intelligence, player.PlayerData.JobData.MaxMpFactor, true);
            int maxFp = HealthFormulas.GetMaxOriginFp(player.Object.Level, stamina, dexterity, strength, player.PlayerData.JobData.MaxFpFactor, true);
            int recoveryHp = HealthFormulas.GetHpRecovery(maxHp, player.Object.Level, stamina, player.PlayerData.JobData.HpRecoveryFactor);
            int recoveryMp = HealthFormulas.GetMpRecovery(maxMp, player.Object.Level, intelligence, player.PlayerData.JobData.MpRecoveryFactor);
            int recoveryFp = HealthFormulas.GetFpRecovery(maxFp, player.Object.Level, stamina, player.PlayerData.JobData.FpRecoveryFactor);

            player.Health.Hp += recoveryHp;
            player.Health.Mp += recoveryMp;
            player.Health.Fp += recoveryFp;

            if (player.Health.Hp > maxHp)
                player.Health.Hp = maxHp;
            if (player.Health.Mp > maxMp)
                player.Health.Mp = maxMp;
            if (player.Health.Fp > maxFp)
                player.Health.Fp = maxFp;

            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.HP, player.Health.Hp);
            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.MP, player.Health.Mp);
            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.FP, player.Health.Fp);
        }
    }
}
