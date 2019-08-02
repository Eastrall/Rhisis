using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Systems.Drop.EventArgs
{
    public class DropItemEventArgs : SystemEventArgs
    {
        public Item Item { get; set; }

        public IWorldEntity Owner { get; set; }

        public DropItemEventArgs(Item item)
            : this(item, null)
        {
        }

        public DropItemEventArgs(Item item, IWorldEntity owner)
        {
            this.Item = item;
            this.Owner = owner;
        }

        public override bool GetCheckArguments()
        {
            return this.Item != null && this.Item.Id > 0;
        }
    }
}
