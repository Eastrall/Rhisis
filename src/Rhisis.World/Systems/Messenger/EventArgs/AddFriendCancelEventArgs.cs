using Rhisis.World.Game.Core;

namespace Rhisis.World.Systems.Messenger.EventArgs
{
    public class AddFriendCancelEventArgs : SystemEventArgs
    {
        public int LeaderId { get; }
        public int MemberId { get; }

        public AddFriendCancelEventArgs(int leaderId, int memberId)
        {
            this.LeaderId = leaderId;
            this.MemberId = memberId;
        }

        public override bool CheckArguments() =>
            this.LeaderId > 0
            && this.MemberId > 0
            && this.LeaderId != this.MemberId;
    }
}