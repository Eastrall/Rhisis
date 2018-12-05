using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Mailbox.EventArgs
{
    public class QueryRemoveMailEventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the id for the mail.
        /// </summary>
        public int MailId { get; }

        public QueryRemoveMailEventArgs(int mailId)
        {
            this.MailId = mailId;
        }

        public override bool CheckArguments()
        {
            return true;
        }
    }
}
