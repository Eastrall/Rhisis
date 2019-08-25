using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.World.Systems.Teleport;
using System.Linq;

namespace Rhisis.World.Systems
{
    [Injectable]
    public sealed class RegionTriggerSystem : IRegionTriggerSystem
    {
        private readonly ITeleportSystem _teleportSystem;

        /// <summary>
        /// Creates a new <see cref="RegionTriggerSystem"/> instance.
        /// </summary>
        /// <param name="teleportSystem">Teleport system.</param>
        public RegionTriggerSystem(ITeleportSystem teleportSystem)
        {
            this._teleportSystem = teleportSystem;
        }

        /// <inheritdoc />
        public void CheckWrapzones(IPlayerEntity player)
        {
            var triggerRegion = player.Object.CurrentMap.Regions.FirstOrDefault(x => x is IMapTriggerRegion region &&
                                                region.IsWrapzone &&
                                                region.GetRectangle().Contains(player.Object.Position))
                       as IMapTriggerRegion;

            if (triggerRegion != null && triggerRegion.IsWrapzone)
            {
                this._teleportSystem.Teleport(player, triggerRegion.DestinationMapId, 
                    triggerRegion.DestinationMapPosition.X, 
                    triggerRegion.DestinationMapPosition.Y, 
                    triggerRegion.DestinationMapPosition.Z);   
            }
        }
    }
}
