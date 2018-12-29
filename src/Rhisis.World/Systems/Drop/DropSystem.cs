using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Drop
{
    [System(SystemType.Notifiable)]
    public class DropSystem : ISystem
    {
        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs args)
        {
        }
    }
}
