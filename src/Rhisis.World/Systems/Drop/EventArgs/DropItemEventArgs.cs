using Rhisis.Core.Data;
using Rhisis.World.Game.Core.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rhisis.World.Systems.Drop.EventArgs
{
    public class DropItemEventArgs : SystemEventArgs
    {
        public int ItemId { get; protected set; }

        public DropItemEventArgs(int itemId)
        {

        }

        public override bool CheckArguments()
        {
            throw new NotImplementedException();
        }
    }
}
