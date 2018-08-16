using Rhisis.World.Game.Core;

namespace Rhisis.World.Systems.Messenger.EventArgs
{
    public class AddFriendCancelEventArgs : SystemEventArgs
    {
        public int SenderId { get; }
        public int ReceiverId { get; }

        public AddFriendCancelEventArgs(int senderId, int receiverId)
        {
            this.SenderId = senderId;
            this.ReceiverId = receiverId;
        }

        public override bool CheckArguments() =>
            this.SenderId > 0
            && this.ReceiverId > 0
            && this.SenderId != this.ReceiverId;
    }
}