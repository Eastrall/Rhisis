using Rhisis.World.Game.Core;

namespace Rhisis.World.Systems.Messenger.EventArgs
{
    public class AddFriendEventArgs : SystemEventArgs
    {
        public int LeaderId { get; }
        public int MemberId { get; }
        public byte LeaderGender { get; }
        public byte MemberGender { get; }
        public int LeaderJob { get; }
        public int MemberJob { get; }

        public AddFriendEventArgs(int leaderId, int memberId, byte leaderGender, byte memberGender, int leaderJob, int memberJob)
        {
            LeaderId = leaderId;
            MemberId = memberId;
            LeaderGender = leaderGender;
            MemberGender = memberGender;
            LeaderJob = leaderJob;
            MemberJob = memberJob;
        }

        // TODO: Check Job and Gender arguments
        public override bool CheckArguments() => LeaderId > 0 && MemberId > 0 && LeaderId != MemberId;
    }
}