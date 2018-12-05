using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Mailbox.EventArgs
{
    public class ReadMailEventArgs : SystemEventArgs
    {

        /// <summary>
        /// Id of the mail.
        /// </summary>
        public int MailId { get; }

        public ReadMailEventArgs(int mailId)
        {
            this.MailId = mailId;
        }

        public override bool CheckArguments()
        {
            return true;
        }
    }
}
