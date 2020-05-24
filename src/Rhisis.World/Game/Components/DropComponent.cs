using Rhisis.Core.Data;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Game.Components
{
    public class DropComponent
    {
        public InventoryItem Item { get; set; }

        public IWorldEntity Owner { get; set; }

        public long OwnershipTime { get; set; }

        public long DespawnTime { get; set; }

        public long RespawnTime { get; set; }

        public bool HasOwner => Owner != null && OwnershipTime > 0;

        public bool IsTemporary => DespawnTime > 0;

        public bool IsGold => Item?.Id == DefineItem.II_GOLD_SEED1 || 
            Item?.Id == DefineItem.II_GOLD_SEED2 || 
            Item?.Id == DefineItem.II_GOLD_SEED3 || 
            Item?.Id == DefineItem.II_GOLD_SEED4;
    }
}
