using Rhisis.Core.Data;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Game.Components
{
    public class DropComponent
    {
        public Item Item { get; set; }

        public IWorldEntity Owner { get; set; }

        public long OwnershipTime { get; set; }

        public long DespawnTime { get; set; }

        public long RespawnTime { get; set; }

        public bool HasOwner => this.Owner != null && this.OwnershipTime > 0;

        public bool IsTemporary => this.DespawnTime > 0;

        public bool IsGold => this.Item?.Id == DefineItem.II_GOLD_SEED1 || 
            this.Item?.Id == DefineItem.II_GOLD_SEED2 || 
            this.Item?.Id == DefineItem.II_GOLD_SEED3 || 
            this.Item?.Id == DefineItem.II_GOLD_SEED4;
    }
}
