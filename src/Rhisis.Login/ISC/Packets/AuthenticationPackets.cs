using Ether.Network.Common;
using Ether.Network.Packets;
using Rhisis.Core.ISC.Packets;
using Rhisis.Core.ISC.Structures;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Login.ISC.Packets
{
    public static class PacketFactory
    {
        public static void SendWelcome(INetUser client)
        {
            using (var packet = new NetPacket())
            {
                packet.Write((uint)InterPacketType.Welcome);

                client.Send(packet);
            }
        }

        public static void SendAuthenticationResult(INetUser client, InterServerCode error)
        {
            using (var packet = new NetPacket())
            {
                packet.Write((uint)InterPacketType.AuthenticationResult);
                packet.Write((uint)error);

                client.Send(packet);
            }
        }

        public static void SendUpdateWorldList(INetUser client, IEnumerable<WorldServerInfo> worlds)
        {
            using (var packet = new NetPacket())
            {
                packet.Write((uint)InterPacketType.UpdateClusterWorldsList);
                packet.Write(worlds.Count());

                foreach (var world in worlds)
                {
                    packet.Write(world.Id);
                    packet.Write(world.Host);
                    packet.Write(world.Name);
                    packet.Write(world.ParentClusterId);
                }

                client.Send(packet);
            }
        }
    }
}
