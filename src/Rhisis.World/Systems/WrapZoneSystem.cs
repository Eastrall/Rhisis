using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.World.Systems.Teleport;
using System.Linq;

namespace Rhisis.World.Systems
{
    /// <summary>
    /// System managing wrapzones.
    /// It detects if a player intersects a wrapzone and then teleport it to the destination map.
    /// </summary>
    [System]
    public sealed class WrapZoneSystem : ISystem
    {
        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (!entity.Object.Spawned)
                return;

            if (entity is IPlayerEntity player)
            {
                var triggerRegion = entity.Object.CurrentMap.Regions.FirstOrDefault(x => x is IMapTriggerRegion region &&
                                             region.IsWrapzone &&
                                             region.GetRectangle().Contains(entity.Object.Position)) 
                    as IMapTriggerRegion;

                if (triggerRegion != null && triggerRegion.IsWrapzone)
                {
                    SystemManager.Instance.Execute<TeleportSystem>(player, new TeleportEventArgs(triggerRegion.DestinationMapId, triggerRegion.DestinationMapPosition));
                }
            }
        }
    }
}
