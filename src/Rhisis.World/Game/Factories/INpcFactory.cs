using Rhisis.Core.Resources.Dyo;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;

namespace Rhisis.World.Game.Factories
{
    public interface INpcFactory
    {
        INpcEntity CreateNpc(IMapContext context, NpcDyoElement element);
    }
}
