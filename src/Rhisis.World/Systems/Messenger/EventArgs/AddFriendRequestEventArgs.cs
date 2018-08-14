using Rhisis.World.Game.Core;

namespace Rhisis.World.Systems.Messenger.EventArgs
{
    public class AddFriendRequestEventArgs : SystemEventArgs
    {
        public int LeaderId { get; }
        public int MemberId { get; }

        public AddFriendRequestEventArgs(int leaderId, int memberId)
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