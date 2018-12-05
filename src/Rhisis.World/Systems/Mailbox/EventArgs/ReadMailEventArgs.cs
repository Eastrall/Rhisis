using Rhisis.World.Game.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rhisis.World.Systems.Mailbox.EventArgs
{
    public class ReadMailEventArgs : SystemEventArgs
    {
        public ReadMailEventArgs()
        {
        }

        public override bool CheckArguments()
        {
            return true;
        }
    }
}
