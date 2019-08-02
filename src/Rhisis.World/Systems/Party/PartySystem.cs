using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Party.EventArgs;

namespace Rhisis.World.Systems.Party
{
    [System(SystemType.Notifiable)]
    public class PartySystem : ISystem
    {
        private readonly ILogger Logger = DependencyContainer.Instance.Resolve<ILogger>();

        public WorldEntityType Type => WorldEntityType.Player;

        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (!(entity is IPlayerEntity player))
                return;

            if(!args.GetCheckArguments())
            {
                Logger.LogError("PartySystem: Invalid event action arguments.");
                return;
            }

            switch(args)
            {
                case PartyMemberRequestEventArgs e:
                    OnPartyMemberRequest(player, e);
                    break;
                case PartyMemberRequestCancelEventArgs e:
                    OnPartyMemberRequestCancel(player, e);
                    break;
                case PartyAddMemberEventArgs e:
                    OnAddPartyMember(player, e);
                    break;
                case PartyRemoveMemberEventArgs e:
                    OnRemovePartyMember(player, e);
                    break;
                default:
                    Logger.LogWarning("Unknown statistics action type: {0} for player {1}", args.GetType(), entity.Object.Name);
                    break;
            }
        }

        private void OnRemovePartyMember(IPlayerEntity player, PartyRemoveMemberEventArgs e)
        {
            var partyManager = DependencyContainer.Instance.Resolve<PartyManager>();
            var worldServer = DependencyContainer.Instance.Resolve<IWorldServer>();

            var member = worldServer.GetPlayerEntityByCharacterId(e.MemberId);
            if (member == null || !member.Party.IsInParty)
                return;
                       
            var party = partyManager.GetPartyById(member.Party.Party.Id);
            if (party == null)
                return;

            if (e.LeaderId != e.MemberId && party.PartyLeaderId != e.LeaderId)
                return;

            if(party.RemoveMember(member))
            {
                if (party.Members.Count < 2)
                {
                    WorldPacketFactory.SendAddPartyMember(party.PartyLeader, null, 0, party.PartyLeader.Object.Name, member.Object.Name);
                    party.PartyLeader.Party.Party = null;
                    partyManager.RemoveParty(party);
                }
                else
                {
                    foreach (var partyMember in party.Members)
                        WorldPacketFactory.SendAddPartyMember(partyMember, party, member.PlayerData.Id, party.PartyLeader.Object.Name, member.Object.Name);
                }

                WorldPacketFactory.SendAddPartyMember(member, null, member.PlayerData.Id, party.PartyLeader.Object.Name, member.Object.Name);
            }            
        }

        private void OnAddPartyMember(IPlayerEntity player, PartyAddMemberEventArgs e)
        {
            var partyManager = DependencyContainer.Instance.Resolve<PartyManager>();
            var worldServer = DependencyContainer.Instance.Resolve<IWorldServer>();

            var leader = worldServer.GetPlayerEntityByCharacterId(e.LeaderId);
            var member = worldServer.GetPlayerEntityByCharacterId(e.MemberId);

            if (leader == null || member == null)
                return;

            if (member.Party.IsInParty)
            {
                Logger.LogWarning($"Player {member.Object.Name} is already in a party.");
                return;
            }

            if(leader.Party.IsInParty)
            {
                // Add member to leaders party
                if(leader.Party.Party.IsFull)
                {
                    // Send SNAPSHOTTYPE_ERRORPARTY with ERROR_FULLPARTY (201)
                    return;
                }

                leader.Party.Party.AddMember(member);

                foreach (var partyMember in leader.Party.Party.Members)
                    WorldPacketFactory.SendAddPartyMember(partyMember, leader.Party.Party, member.PlayerData.Id, leader.Object.Name, member.Object.Name);
            }
            else
            {
                // Create Party with leader and member
                var party = partyManager.CreateParty();
                party.AddMember(leader, true);
                party.AddMember(member);

                foreach (var partyMember in party.Members)
                    WorldPacketFactory.SendAddPartyMember(partyMember, party, member.PlayerData.Id, leader.Object.Name, member.Object.Name);
            }
        }

        private void OnPartyMemberRequestCancel(IPlayerEntity player, PartyMemberRequestCancelEventArgs e)
        {
            var worldServer = DependencyContainer.Instance.Resolve<IWorldServer>();

            var leader = worldServer.GetPlayerEntityByCharacterId(e.LeaderId);
            if (leader == null)
                return;

            WorldPacketFactory.SendAddPartyRequestCancel(leader, e.MemberId, e.Mode);
        }

        private void OnPartyMemberRequest(IPlayerEntity player, PartyMemberRequestEventArgs e)
        {
            var worldServer = DependencyContainer.Instance.Resolve<IWorldServer>();

            if (player.PlayerData.Id != e.PlayerId)
                return;

            var member = worldServer.GetPlayerEntityByCharacterId(e.MemberId);
            if (member == null)
            {
                WorldPacketFactory.SendAddPartyRequestCancel(player, e.MemberId, PartyRequestCancelMode.OtherServer);
                return;
            }

            // TODO: Check if in GW, Duel or other stuff that might disturb the player.

            if(member.Party.IsInParty)
            {
                WorldPacketFactory.SendAddPartyRequestCancel(player, e.MemberId, PartyRequestCancelMode.OtherParty);
                return;
            }

            // Send Request to other player
            WorldPacketFactory.SendAddPartyRequest(player, member, e.Troup);
        }
    }
}
