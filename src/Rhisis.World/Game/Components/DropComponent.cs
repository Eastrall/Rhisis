using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Game.Components
{
    public class DropComponent
    {
        public Item Item { get; set; }

        public IPlayerEntity Owner { get; set; }
    }
}
