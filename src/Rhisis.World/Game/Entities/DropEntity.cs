using Rhisis.Core.Common;
using Rhisis.World.Game.Core;

namespace Rhisis.World.Game.Entities
{
    public class DropEntity : Entity, IDropEntity
    {
        public override WorldEntityType Type => WorldEntityType.Drop;

        public DropEntity(IContext context) 
            : base(context)
        {
            this.Object.Type = WorldObjectType.Item;
        }
    }
}
