using Rhisis.World.Game.Core;

namespace Rhisis.World.Systems.Messenger.EventArgs
{
    public class AddFriendNameRequestEventArgs : SystemEventArgs
    {
        public int LeaderId { get; }
        public string MemberName { get; }

        public AddFriendNameRequestEventArgs(int leaderId, string memberName)
        {
            this.LeaderId = leaderId;
            this.MemberName = memberName;
        }

        public override bool CheckArguments() =>
            this.LeaderId > 0
            && !string.IsNullOrWhiteSpace(this.MemberName);
    }
}