using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Mailbox.EventArgs
{
    public class QueryMailboxEventArgs : SystemEventArgs
    {
        public QueryMailboxEventArgs()
        {
        }

        public override bool CheckArguments()
        {
            return true;
        }
    }
}
