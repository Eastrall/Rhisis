using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Systems.Statistics;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.World.Handlers
{
    [Handler]
    public class StatisticsHandler
    {
        private readonly IStatisticsSystem _statisticsSystem;

        /// <summary>
        /// Creates a new <see cref="StatisticsHandler"/> instance.
        /// </summary>
        /// <param name="statisticsSystem">Statistics system.</param>
        public StatisticsHandler(IStatisticsSystem statisticsSystem)
        {
            this._statisticsSystem = statisticsSystem;
        }

        /// <summary>
        /// Handles the MODIFY_STATUS for updating a player's statistics.
        /// </summary>
        /// <param name="client">Current client.</param>
        /// <param name="packet">Incoming packet.</param>
        [HandlerAction(PacketType.MODIFY_STATUS)]
        public void OnModifyStatus(IWorldClient client, ModifyStatusPacket packet)
        {
            this._statisticsSystem.UpdateStatistics(client.Player, 
                packet.Strenght, packet.Stamina, packet.Dexterity, packet.Intelligence);
        }
    }
}
