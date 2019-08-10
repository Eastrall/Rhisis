using Rhisis.Core.Resources.Dyo;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;

namespace Rhisis.World.Game.Factories
{
    public interface INpcFactory
    {
        /// <summary>
        /// Creates a new NPC.
        /// </summary>
        /// <param name="context">NPC map context.</param>
        /// <param name="element">Npc DYO element.</param>
        /// <returns>New NPC.</returns>
        INpcEntity CreateNpc(IMapContext context, NpcDyoElement element);
    }
}
