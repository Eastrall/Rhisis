using Microsoft.Extensions.Logging;
using Rhisis.Core.Common.Formulas;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.IO;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Recovery.EventArgs;

namespace Rhisis.World.Systems.Recovery
{
    /// <summary>
    /// Game system that manages all recoveries. HP, MP, FP, etc...
    /// </summary>
    [System(SystemType.Notifiable)]
    public sealed class RecoverySystem : ISystem
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

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;
        
        /// <summary>
        /// Creates a new <see cref="RecoverySystem"/> instance.
        /// </summary>
        public RecoverySystem()
        {
            this._logger = DependencyContainer.Instance.Resolve<ILogger<RecoverySystem>>();
        }

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (!(entity is IPlayerEntity player))
            {
                this._logger.LogError($"Cannot execute {nameof(RecoverySystem)}. {entity.Object.Name} is not a player.");
                return;
            }

            if (!args.GetCheckArguments())
            {
                this._logger.LogError($"Cannot execute {nameof(RecoverySystem)} action: {args.GetType()} due to invalid arguments.");
                return;
            }

            switch (args)
            {
                case IdleRecoveryEventArgs e:
                    this.IdleHeal(player, e);
                    break;
                default:
                    this._logger.LogWarning($"Unknown recovery system action type: {args.GetType()} for player {entity.Object.Name}");
                    break;
            }
        }

        /// <summary>
        /// Process the idle heal.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="e">Idle recovery event args.</param>
        private void IdleHeal(IPlayerEntity player, IdleRecoveryEventArgs e)
        {
            int nextHealDelay = e.IsSitted ? NextIdleHealSit : NextIdleHealStand;

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
