using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;

namespace Rhisis.World.Systems.Statistics
{
    [Injectable]
    public sealed class StatisticsSystem : IStatisticsSystem
    {
        private readonly ILogger<StatisticsSystem> _logger;
        private readonly IPlayerPacketFactory _playerPacketFactory;

        /// <summary>
        /// Creates a new <see cref="StatisticsSystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public StatisticsSystem(ILogger<StatisticsSystem> logger, IPlayerPacketFactory playerPacketFactory)
        {
            this._logger = logger;
            this._playerPacketFactory = playerPacketFactory;
        }

        /// <inheritdoc />
        public void UpdateStatistics(IPlayerEntity player, ushort strength, ushort stamina, ushort dexterity, ushort intelligence)
        {
            this._logger.LogDebug("Modify sttus");

            var total = strength + stamina + dexterity + intelligence;

            var statsPoints = player.Statistics.StatPoints;
            if (statsPoints <= 0 || total > statsPoints)
            {
                this._logger.LogError("No statspoints available, but trying to upgrade {0}.", player.Object.Name);
                return;
            }

            if (strength > statsPoints || stamina > statsPoints ||
                dexterity > statsPoints || intelligence > statsPoints || total <= 0 ||
                total > ushort.MaxValue)
            {
                this._logger.LogError("Invalid upgrade request due to bad total calculation (trying to dupe) {0}.",
                    player.Object.Name);
                return;
            }

            player.Attributes.IncreaseAttribute(DefineAttributes.STR, strength);
            player.Attributes.IncreaseAttribute(DefineAttributes.STA, stamina);
            player.Attributes.IncreaseAttribute(DefineAttributes.DEX, dexterity);
            player.Attributes.IncreaseAttribute(DefineAttributes.INT, intelligence);
            player.Statistics.StatPoints -= (ushort)total;

            this._playerPacketFactory.SendPlayerUpdateState(player);
        }
    }
}
