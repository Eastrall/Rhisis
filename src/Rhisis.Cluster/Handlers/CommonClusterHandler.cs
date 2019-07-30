using Rhisis.Cluster.Client;
using Rhisis.Cluster.Packets;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.Cluster;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.Cluster.Handlers
{
    /// <summary>
    /// Provides common handlers for the cluster server.
    /// </summary>
    [Handler]
    public class CommonClusterHandler
    {
        private readonly IClusterPacketFactory _clusterPacketFactory;

        /// <summary>
        /// Creates a new <see cref="CommonClusterHandler"/> instance.
        /// </summary>
        /// <param name="clusterPacketFactory">Cluster packet factory.</param>
        public CommonClusterHandler(IClusterPacketFactory clusterPacketFactory)
        {
            this._clusterPacketFactory = clusterPacketFactory;
        }

        /// <summary>
        /// Handles a ping request.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="pingPacket">Ping packet data.</param>
        [HandlerAction(PacketType.PING)]
        public void OnPing(IClusterClient client, PingPacket pingPacket)
        {
            if (!pingPacket.IsTimeOut)
                this._clusterPacketFactory.SendPong(client, pingPacket.Time);
        }

        /// <summary>
        /// Handles a query tick count request.
        /// </summary>
        /// <param name="client">Client.</param>
        /// <param name="packet">Query tick count packet data.</param>
        [HandlerAction(PacketType.QUERYTICKCOUNT)]
        public void OnQueryTickCount(IClusterClient client, QueryTickCountPacket packet)
        {
            this._clusterPacketFactory.SendQueryTickCount(client, packet.Time);
        }
    }
}
