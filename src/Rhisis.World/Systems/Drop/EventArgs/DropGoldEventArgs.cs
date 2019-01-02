using Rhisis.Core.Structures;
using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Drop.EventArgs
{
    public class DropGoldEventArgs : SystemEventArgs
    {
        public int GoldAmount { get; }
        
        public DropGoldEventArgs(int amount)
        {
            this.GoldAmount = amount;
        }

        public override bool CheckArguments() => this.GoldAmount > 0;
    }
}
