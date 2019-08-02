using NLog;
using Rhisis.Core.Data;
using Rhisis.Core.IO;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;

namespace Rhisis.World.Systems.Statistics
{
    [System(SystemType.Notifiable)]
    public class StatisticsSystem : ISystem
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity playerEntity) ||
                !e.GetCheckArguments())
            {
                Logger.Error("StatisticsSystem: Invalid event action arguments.");
                return;
            }

            Logger.Debug("Execute statistics action: {0}", e.GetType());

            switch (e)
            {
                case StatisticsModifyEventArgs statisticsModifyEvent:
                    this.ModifyStatus(playerEntity, statisticsModifyEvent);
                    break;
                default:
                    Logger.Warn("Unknown statistics action type: {0} for player {1}", e.GetType(), entity.Object.Name);
                    break;
            }

            WorldPacketFactory.SendUpdateState(playerEntity);
        }

        private void ModifyStatus(IPlayerEntity player, StatisticsModifyEventArgs e)
        {
            Logger.Debug("Modify sttus");

            var total = e.Strenght + e.Stamina + e.Dexterity + e.Intelligence;

            var statsPoints = player.Statistics.StatPoints;
            if (statsPoints <= 0 || total > statsPoints)
            {
                Logger.Error("No statspoints available, but trying to upgrade {0}.", player.Object.Name);
                return;
            }

            if (e.Strenght > statsPoints || e.Stamina > statsPoints ||
                e.Dexterity > statsPoints || e.Intelligence > statsPoints || total <= 0 ||
                total > ushort.MaxValue)
            {
                Logger.Error("Invalid upgrade request due to bad total calculation (trying to dupe) {0}.",
                    player.Object.Name);
                return;
            }

            player.Attributes.IncreaseAttribute(DefineAttributes.STR, e.Strenght);
            player.Attributes.IncreaseAttribute(DefineAttributes.STA, e.Stamina);
            player.Attributes.IncreaseAttribute(DefineAttributes.DEX, e.Dexterity);
            player.Attributes.IncreaseAttribute(DefineAttributes.INT, e.Intelligence);
            player.Statistics.StatPoints -= (ushort) total;
        }
    }
}
