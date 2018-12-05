using Rhisis.World.Game.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rhisis.World.Systems.Mailbox.EventArgs
{
    class QueryRemoveMailEventArgs : SystemEventArgs
    {
        public QueryRemoveMailEventArgs()
        {
        }

        public override bool CheckArguments()
        {
            return true;
        }
    }
}
