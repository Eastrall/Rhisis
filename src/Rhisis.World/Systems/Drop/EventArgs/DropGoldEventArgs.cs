using Rhisis.Core.Structures;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Drop.EventArgs
{
    public class DropGoldEventArgs : SystemEventArgs
    {
        public int GoldAmount { get; }

        public IEntity Owner { get; }
        
        public DropGoldEventArgs(int amount, IEntity owner)
        {
            this.GoldAmount = amount;
            this.Owner = owner;
        }

        public override bool CheckArguments() => this.GoldAmount > 0;
    }
}
