using Ether.Network.Client;
using Ether.Network.Packets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Network.Core;
using Sylver.HandlerInvoker;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Rhisis.Cluster.CoreClient
{
    public sealed class ClusterCoreClient : NetClient, IClusterCoreClient
    {
        private readonly ILogger<ClusterCoreClient> _logger;
        private readonly ISCConfiguration _coreConfiguration;
        private readonly IHandlerInvoker _handlerInvoker;

        /// <inheritdoc />
        public ClusterConfiguration ClusterConfiguration { get; }

        /// <inheritdoc />
        public IList<WorldServerInfo> WorldServers { get; }

        /// <summary>
        /// Gets the remote end point (IP and port) for this client.
        /// </summary>
        public string RemoteEndPoint => this.Socket.RemoteEndPoint.ToString();

        /// <summary>
        /// Creates a new <see cref="ClusterCoreClient"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="clusterConfiguration">Cluster server configuration.</param>
        /// <param name="coreConfiguration">Core server configuration.</param>
        /// <param name="handlerInvoker">Handler invoker.</param>
        public ClusterCoreClient(ILogger<ClusterCoreClient> logger, IOptions<ClusterConfiguration> clusterConfiguration, IOptions<ISCConfiguration> coreConfiguration, IHandlerInvoker handlerInvoker)
        {
            this._logger = logger;
            this._coreConfiguration = coreConfiguration.Value;
            this.ClusterConfiguration = clusterConfiguration.Value;
            this._handlerInvoker = handlerInvoker;
            this.Configuration.Host = this._coreConfiguration.Host;
            this.Configuration.Port = this._coreConfiguration.Port;
            this.Configuration.BufferSize = 128;
            this.WorldServers = new List<WorldServerInfo>();
        }

        /// <inheritdoc />
        protected override void OnConnected()
        {
            this._logger.LogInformation("Cluster core client connected to core server.");
        }

        /// <inheritdoc />
        protected override void OnDisconnected()
        {
            this._logger.LogInformation("Disconnected from core server.");

            // TODO: try to reconnect.
        }

        /// <inheritdoc />
        protected override void OnSocketError(SocketError socketError)
        {
            this._logger.LogError($"An error occured on Cluster core client: {socketError}");
        }

        /// <inheritdoc />
        public override void HandleMessage(INetPacketStream packet)
        {
            uint packetHeaderNumber = 0;

            if (this.Socket == null)
            {
                this._logger.LogError("Skip to handle core packet from server. Reason: socket is not connected.");
                return;
            }

            try
            {
                packetHeaderNumber = packet.Read<uint>();
                this._handlerInvoker.Invoke((CorePacketType)packetHeaderNumber, this, packet);
            }
            catch (ArgumentNullException)
            {
                if (Enum.IsDefined(typeof(CorePacketType), packetHeaderNumber))
                    this._logger.LogWarning("Received an unimplemented Core packet {0} (0x{1}) from {2}.", Enum.GetName(typeof(CorePacketType), packetHeaderNumber), packetHeaderNumber.ToString("X4"), this.RemoteEndPoint);
                else
                    this._logger.LogWarning("[SECURITY] Received an unknown Core packet 0x{0} from {1}.", packetHeaderNumber.ToString("X4"), this.RemoteEndPoint);
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, $"An error occured while handling a core packet.");
                this._logger.LogDebug(exception.InnerException?.StackTrace);
            }
        }
    }
}
