using Rhisis.World.Game.Maps.Regions;
using System.Collections.Generic;

namespace Rhisis.World.Game.Maps
{
    public sealed class MapLayer : MapContext, IMapLayer
    {
        public IMapInstance ParentMap { get; }

        public ICollection<IMapRegion> Regions { get; }

        public MapLayer(IMapInstance parentMapInstance, int layerId)
        {
            this.Id = layerId;
            this.ParentMap = parentMapInstance;
        }
    }
}
