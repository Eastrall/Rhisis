using Ether.Network.Packets;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World.Party;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Systems.Party;
using Rhisis.World.Systems.Party.EventArgs;

namespace Rhisis.World.Handlers
{
    internal static class PartyHandler
    {
        [PacketHandler(PacketType.MEMBERREQUEST)]
        public static void OnPartyMemberRequest(WorldClient client, INetPacketStream packet)
        {
            var partyMemberRequestPacket = new PartyMemberRequestPacket(packet);
            var partyMemberRequestEventArgs = new PartyMemberRequestEventArgs(partyMemberRequestPacket);

            SystemManager.Instance.Execute<PartySystem>(client.Player, partyMemberRequestEventArgs);
        }

        [PacketHandler(PacketType.MEMBERREQUESTCANCLE)]
        public static void OnPartyMemberRequestCancel(WorldClient client, INetPacketStream packet)
        {
            var partyMemberRequestCancelPacket = new PartyMemberRequestCancelPacket(packet);
            var partyMemberRequestCancelEventArgs = new PartyMemberRequestCancelEventArgs(partyMemberRequestCancelPacket);

            SystemManager.Instance.Execute<PartySystem>(client.Player, partyMemberRequestCancelEventArgs);
        }

        [PacketHandler(PacketType.ADDPARTYMEMBER)]
        public static void OnAddPartyMember(WorldClient client, INetPacketStream packet)
        {
            var addPartyMemberPacket = new PartyAddMemberPacket(packet);
            var addPartyMemberEventArgs = new PartyAddMemberEventArgs(addPartyMemberPacket);

            SystemManager.Instance.Execute<PartySystem>(client.Player, addPartyMemberEventArgs);
        }

        [PacketHandler(PacketType.REMOVEPARTYMEMBER)]
        public static void OnRemovePartyMember(WorldClient client, INetPacketStream packet)
        {
            var removePartyMemberPacket = new PartyRemoveMemberPacket(packet);
            var removePartyMemberEventArgs = new PartyRemoveMemberEventArgs(removePartyMemberPacket);

            SystemManager.Instance.Execute<PartySystem>(client.Player, removePartyMemberEventArgs);
        }
    }
}
