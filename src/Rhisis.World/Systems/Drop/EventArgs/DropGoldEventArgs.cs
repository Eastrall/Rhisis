using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Drop.EventArgs
{
    public class DropGoldEventArgs : SystemEventArgs
    {
        public int GoldAmount { get; }

        public IWorldEntity Owner { get; }
        
        public DropGoldEventArgs(int amount, IWorldEntity owner)
        {
            this.GoldAmount = amount;
            this.Owner = owner;
        }

        public override bool GetCheckArguments()
        {
            return this.GoldAmount > 0;
        }
    }
}
