using Ether.Network.Common;
using Ether.Network.Packets;
using Rhisis.Core.Helpers;
using Rhisis.Network.Packets;
using System;
using Microsoft.Extensions.Logging;
using Rhisis.Cluster.Packets;
using Sylver.HandlerInvoker;

namespace Rhisis.Cluster.Client
{
    public sealed class ClusterClient : NetUser, IClusterClient
    {
        private ILogger<ClusterClient> _logger;
        private IClusterServer _clusterServer;
        private IHandlerInvoker _handlerInvoker;

        /// <inheritdoc />
        public uint SessionId { get; }

        /// <inheritdoc />
        public int LoginProtectValue { get; set; }

        /// <inheritdoc />
        public string RemoteEndPoint => this.Socket.RemoteEndPoint.ToString();

        /// <summary>
        /// Creates a new <see cref="ClusterClient"/> instance.
        /// </summary>
        public ClusterClient()
        {
            this.SessionId = RandomHelper.GenerateSessionKey();
            this.LoginProtectValue = new Random().Next(0, 1000);
        }

        /// <summary>
        /// Initializes the <see cref="ClusterClient"/>.
        /// </summary>
        /// <param name="clusterServer">Parent cluster server.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="handlerInvoker">Handler invoker.</param>
        /// <param name="clusterPacketFactory">Cluster packet factory.</param>
        public void Initialize(IClusterServer clusterServer, ILogger<ClusterClient> logger, IHandlerInvoker handlerInvoker, IClusterPacketFactory clusterPacketFactory)
        {
            this._clusterServer = clusterServer;
            this._logger = logger;
            this._handlerInvoker = handlerInvoker;

            clusterPacketFactory.SendWelcome(this);
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            this.Dispose();
            this._clusterServer.DisconnectClient(this.Id);
        }

        /// <summary>
        /// Send a packet to the client.
        /// </summary>
        /// <param name="packet"></param>
        public override void Send(INetPacketStream packet)
        {
            this._logger.LogTrace("Send {0} packet to {1}.",
                    (PacketType)BitConverter.ToUInt32(packet.Buffer, 5),
                    this.RemoteEndPoint);

            base.Send(packet);
        }

        /// <summary>
        /// Handle the incoming mesages.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public override void HandleMessage(INetPacketStream packet)
        {
            uint packetHeaderNumber = 0;

            if (this.Socket == null)
            {
                this._logger.LogTrace("Skip to handle cluster packet from {0}. Reason: client is not connected.", this.RemoteEndPoint);
                return;
            }

            try
            {
                packet.Read<uint>(); // DPID: Always 0xFFFFFFFF (uint.MaxValue)
                packetHeaderNumber = packet.Read<uint>();

#if DEBUG
                this._logger.LogTrace("Received {0} packet from {1}.", (PacketType)packetHeaderNumber, this.RemoteEndPoint);
#endif
                this._handlerInvoker.Invoke((PacketType)packetHeaderNumber, this, packet);
            }
            catch (ArgumentNullException)
            {
                if (Enum.IsDefined(typeof(PacketType), packetHeaderNumber))
                    this._logger.LogWarning("Received an unimplemented Cluster packet {0} (0x{1}) from {2}.", Enum.GetName(typeof(PacketType), packetHeaderNumber), packetHeaderNumber.ToString("X4"), this.RemoteEndPoint);
                else
                    this._logger.LogWarning("[SECURITY] Received an unknown Cluster packet 0x{0} from {1}.", packetHeaderNumber.ToString("X4"), this.RemoteEndPoint);
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, $"An error occured while handling a cluster packet.");
                this._logger.LogDebug(exception.InnerException?.StackTrace);
            }
        }
    }
}
