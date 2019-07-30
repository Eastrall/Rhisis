using Ether.Network.Common;
using Ether.Network.Packets;
using Microsoft.Extensions.Logging;
using Rhisis.Network.Core;
using Sylver.HandlerInvoker;
using System;

namespace Rhisis.Login.Core
{
    public class CoreServerClient : NetUser, ICoreServerClient
    {
        private ILogger<CoreServerClient> _logger;
        private IHandlerInvoker _handlerInvoker;

        /// <summary>
        /// Gets the remote end point (IP and port).
        /// </summary>
        public string RemoteEndPoint { get; private set; }

        /// <summary>
        /// Gets or sets the server informations.
        /// </summary>
        public ServerInfo ServerInfo { get; internal set; }

        /// <summary>
        /// Initialize the <see cref="CoreServerClient"/>.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="handlerInvoker">Handler Invoker.</param>
        public void Initialize(ILogger<CoreServerClient> logger, IHandlerInvoker handlerInvoker)
        {
            this._logger = logger;
            this._handlerInvoker = handlerInvoker;
            this.RemoteEndPoint = this.Socket.RemoteEndPoint.ToString();
        }

        /// <inheritdoc />
        public override void HandleMessage(INetPacketStream packet)
        {
            uint packetHeaderNumber = 0;

            if (this.Socket == null)
            {
                this._logger.LogTrace("Skip to handle core packet from {0}. Reason: socket is not connected.", this.RemoteEndPoint);
                return;
            }

            try
            {
                packetHeaderNumber = packet.Read<uint>();
                this._handlerInvoker.Invoke((CorePacketType)packetHeaderNumber, this, packet);
            }
            catch (ArgumentException)
            {
                if (Enum.IsDefined(typeof(CorePacketType), packetHeaderNumber))
                {
                    this._logger.LogWarning("Received an unimplemented Core packet {0} (0x{1}) from {2}.",
                        Enum.GetName(typeof(CorePacketType), packetHeaderNumber),
                        packetHeaderNumber.ToString("X2"),
                        this.RemoteEndPoint);
                }
                else
                {
                    this._logger.LogWarning($"Received an unknown Core packet 0x{packetHeaderNumber.ToString("X2")} from {this.RemoteEndPoint}.");
                }
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, $"An error occured while handling a CoreServer packet.");
                this._logger.LogDebug(exception.InnerException?.StackTrace);
            }
        }
    }
}
