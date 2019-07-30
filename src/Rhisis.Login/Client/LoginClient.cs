using Ether.Network.Common;
using Ether.Network.Packets;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Helpers;
using Rhisis.Login.Packets;
using Rhisis.Network.Packets;
using Sylver.HandlerInvoker;
using System;

namespace Rhisis.Login.Client
{
    public sealed class LoginClient : NetUser, ILoginClient
    {
        private ILogger<LoginClient> _logger;
        private ILoginServer _loginServer;
        private IHandlerInvoker _handlerInvoker;

        /// <inheritdoc />
        public uint SessionId { get; }

        /// <inheritdoc />
        public string Username { get; private set; }

        /// <inheritdoc />
        public string RemoteEndPoint => this.Socket?.RemoteEndPoint?.ToString();

        /// <inheritdoc />
        public bool IsConnected => !string.IsNullOrEmpty(this.Username);

        /// <summary>
        /// Creates a new <see cref="LoginClient"/> instance.
        /// </summary>
        public LoginClient()
        {
            this.SessionId = RandomHelper.GenerateSessionKey();
        }

        /// <summary>
        /// Initializes the current <see cref="LoginClient"/> instance.
        /// </summary>
        /// <param name="loginServer">Parent login server.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="handlerInvoker">Handler invoker.</param>
        /// <param name="loginPacketFactory">Login packet factory.</param>
        public void Initialize(ILoginServer loginServer, ILogger<LoginClient> logger, IHandlerInvoker handlerInvoker, ILoginPacketFactory loginPacketFactory)
        {
            this._loginServer = loginServer;
            this._logger = logger;
            this._handlerInvoker = handlerInvoker;

            loginPacketFactory.SendWelcome(this, this.SessionId);
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            this._loginServer.DisconnectClient(this.Id);
            this.Dispose();
        }

        /// <inheritdoc />
        public void SetClientUsername(string username)
        {
            if (!string.IsNullOrEmpty(this.Username))
                throw new InvalidOperationException("Client username already set.");

            this.Username = username;
        }

        /// <inheritdoc />
        public override void Send(INetPacketStream packet)
        {
            this._logger.LogTrace("Send {0} packet to {1}.", (PacketType)BitConverter.ToUInt32(packet.Buffer, 5), this.RemoteEndPoint);
            base.Send(packet);
        }

        /// <inheritdoc />
        public override void HandleMessage(INetPacketStream packet)
        {
            uint packetHeaderNumber = 0;

            if (Socket == null)
            {
                this._logger.LogTrace("Skip to handle login packet. Reason: client is not connected.");
                return;
            }

            try
            {
                packetHeaderNumber = packet.Read<uint>();

                this._logger.LogTrace("Received {0} packet from {1}.", (PacketType)packetHeaderNumber, this.RemoteEndPoint);
                this._handlerInvoker.Invoke((PacketType)packetHeaderNumber, this, packet);
            }
            catch (ArgumentException)
            {
                if (Enum.IsDefined(typeof(PacketType), packetHeaderNumber))
                {
                    this._logger.LogWarning("Received an unimplemented Login packet {0} (0x{1}) from {2}.",
                        Enum.GetName(typeof(PacketType), packetHeaderNumber),
                        packetHeaderNumber.ToString("X2"),
                        this.RemoteEndPoint);
                }
                else
                {
                    this._logger.LogWarning("Received an unknown Login packet 0x{0} from {1}.", 
                        packetHeaderNumber.ToString("X2"), 
                        this.RemoteEndPoint);
                }
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, $"An error occured while handling a login packet.");
                this._logger.LogDebug(exception.InnerException?.StackTrace);
            }
        }
    }
}