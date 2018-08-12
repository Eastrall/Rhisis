using Ether.Network.Packets;
using NLog;
using Rhisis.Core.ISC.Packets;
using Rhisis.Core.Network;

namespace Rhisis.World.ISC
{
    public static class ISCHandler
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        [PacketHandler(InterPacketType.Welcome)]
        public static void OnWelcome(ISCClient client, INetPacketStream packet)
        {
            ISCPackets.SendAuthentication(client, client.WorldConfiguration);
        }

        [PacketHandler(InterPacketType.AuthenticationResult)]
        public static void OnAuthenticationResult(ISCClient client, INetPacketStream packet)
        {
            var authenticationResult = packet.Read<uint>();

            Logger.Debug("Authentication result: {0}", (InterServerCode)authenticationResult);
        }
    }
}
