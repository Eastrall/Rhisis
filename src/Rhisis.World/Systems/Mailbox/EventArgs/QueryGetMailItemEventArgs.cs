using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Mailbox.EventArgs
{
    public class QueryGetMailItemEventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the id of the mail
        /// </summary>
        public int MailId { get; }

        public QueryGetMailItemEventArgs(int mailId)
        {
            this.MailId = mailId;
        }

        public override bool CheckArguments()
        {
            return true;
        }
    }
}
