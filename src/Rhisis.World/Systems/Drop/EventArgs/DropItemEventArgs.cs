using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Drop.EventArgs
{
    public class DropItemEventArgs : SystemEventArgs
    {
        public DropItemData Item { get; set; }

        public IEntity Owner { get; set; }

        public DropItemEventArgs(DropItemData itemData)
            : this(itemData, null)
        {
        }

        public DropItemEventArgs(DropItemData itemData, IEntity owner)
        {
            this.Item = itemData;
            this.Owner = owner;
        }

        public override bool CheckArguments() => this.Item != null && this.Item.ItemId > 0;
    }
}
