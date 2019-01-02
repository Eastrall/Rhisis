using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;

namespace Rhisis.World.Game.Entities
{
    public interface IItemEntity : IEntity
    {
        DropComponent Drop { get; set; }
    }
}
