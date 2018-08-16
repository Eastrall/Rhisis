using Rhisis.World.Game.Core;

namespace Rhisis.World.Systems.Messenger.EventArgs
{
    public class AddFriendEventArgs : SystemEventArgs
    {
        public int SenderId { get; }
        public int ReceiverId { get; }
        public byte SenderGender { get; }
        public byte ReceiverGender { get; }
        public int SenderJob { get; }
        public int ReceiverJob { get; }

        public AddFriendEventArgs(int senderId, int receiverId, byte senderGender, byte receiverGender, int senderJob, int receiverJob)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            SenderGender = senderGender;
            ReceiverGender = receiverGender;
            SenderJob = senderJob;
            ReceiverJob = receiverJob;
        }

        // TODO: Check Job and Gender arguments
        public override bool CheckArguments() => SenderId > 0 && ReceiverId > 0 && SenderId != ReceiverId;
    }
}