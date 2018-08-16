using Rhisis.World.Game.Core;

namespace Rhisis.World.Systems.Messenger.EventArgs
{
    public class AddFriendNameRequestEventArgs : SystemEventArgs
    {
        public int SenderId { get; }
        public string ReceiverId { get; }

        public AddFriendNameRequestEventArgs(int senderId, string receiverId)
        {
            this.SenderId = senderId;
            this.ReceiverId = receiverId;
        }

        public override bool CheckArguments() =>
            this.SenderId > 0
            && !string.IsNullOrWhiteSpace(this.ReceiverId);
    }
}