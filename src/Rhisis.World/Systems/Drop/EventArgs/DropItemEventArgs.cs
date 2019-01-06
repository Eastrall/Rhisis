using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Systems.Drop.EventArgs
{
    public class DropItemEventArgs : SystemEventArgs
    {
        public ItemBase Item { get; set; }

        public IEntity Owner { get; set; }

        public DropItemEventArgs(ItemBase item)
            : this(item, null)
        {
        }

        public DropItemEventArgs(ItemBase item, IEntity owner)
        {
            this.Item = item;
            this.Owner = owner;
        }

        public override bool CheckArguments() => this.Item != null && this.Item.Id > 0;
    }
}
