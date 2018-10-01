using Ether.Network.Common;
using Ether.Network.Packets;
using NLog;
using Rhisis.Core.Exceptions;
using Rhisis.Core.Helpers;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Network;
using Rhisis.Network.ISC.Structures;
using Rhisis.Network.Packets;
using System;
using System.Collections.Generic;

namespace Rhisis.Login
{
    public sealed class LoginClient : NetUser
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private LoginServer _loginServer;

        /// <summary>
        /// Gets the ID assigned to this session.
        /// </summary>
        public uint SessionId { get; }

        /// <summary>
        /// Gets the list of connected clusters.
        /// </summary>
        public IEnumerable<ClusterServerInfo> ClustersConnected => this._loginServer.ClustersConnected;

        /// <summary>
        /// Gets the login server's configuration.
        /// </summary>
        public LoginConfiguration ServerConfiguration => this._loginServer.LoginConfiguration;

        /// <summary>
        /// Gets the remote end point (IP and port) for this client.
        /// </summary>
        public string RemoteEndPoint { get; private set; }

        /// <summary>
        /// Creates a new <see cref="LoginClient"/> instance.
        /// </summary>
        public LoginClient()
        {
            this.SessionId = RandomHelper.GenerateSessionKey();
        }

        /// <summary>
        /// Initialize the client.
        /// </summary>
        /// <param name="loginServer"></param>
        public void Initialize(LoginServer loginServer)
        {
            this._loginServer = loginServer;
            this.RemoteEndPoint = this.Socket.RemoteEndPoint.ToString();
        }

        /// <summary>
        /// Disconnects the current client.
        /// </summary>
        public void Disconnect()
        {
            this._loginServer.DisconnectClient(this.Id);
            this.Dispose();
        }

        /// <inheritdoc />
        public override void Send(INetPacketStream packet)
        {
            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("Send {0} packet to {1}.",
                    (PacketType)BitConverter.ToUInt32(packet.Buffer, 5),
                    this.RemoteEndPoint);
            }

            base.Send(packet);
        }

        /// <inheritdoc />
        public override void HandleMessage(INetPacketStream packet)
        {
            FFPacket pak = null;
            uint packetHeaderNumber = 0;

            if (Socket == null)
            {
                Logger.Trace("Skip to handle packet from {0}. Reason: client is no more connected.", this.RemoteEndPoint);
                return;
            }

            try
            {
                pak = packet as FFPacket;
                packetHeaderNumber = packet.Read<uint>();

                if (Logger.IsTraceEnabled)
                    Logger.Trace("Received {0} packet from {1}.", (PacketType)packetHeaderNumber, this.RemoteEndPoint);

                PacketHandler<LoginClient>.Invoke(this, pak, (PacketType)packetHeaderNumber);
            }
            catch (KeyNotFoundException)
            {
                if (Enum.IsDefined(typeof(PacketType), packetHeaderNumber))
                    Logger.Warn("Received an unimplemented Login packet {0} (0x{1}) from {2}.", Enum.GetName(typeof(PacketType), packetHeaderNumber), packetHeaderNumber.ToString("X2"), this.RemoteEndPoint);
                else
                    Logger.Warn("Received an unknown Login packet 0x{0} from {1}.", packetHeaderNumber.ToString("X2"), this.RemoteEndPoint);
            }
            catch (RhisisPacketException packetException)
            {
                Logger.Error("Packet handle error from {0}. {1}", this.RemoteEndPoint, packetException);
                Logger.Debug(packetException.InnerException?.StackTrace);
            }
        }
    }
}