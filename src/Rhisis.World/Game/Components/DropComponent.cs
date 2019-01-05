using Rhisis.World.Game.Core;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Game.Components
{
    public class DropComponent
    {
        public Item Item { get; set; }

        public IEntity Owner { get; set; }

        public long OwnershipTime { get; set; }

        public long DespawnTime { get; set; }

        public bool HasOwner => this.Owner != null && this.OwnershipTime > 0;
    }
}
