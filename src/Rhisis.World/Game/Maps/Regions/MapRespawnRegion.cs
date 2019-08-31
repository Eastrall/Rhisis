using Rhisis.Core.Common;
using Rhisis.Core.Resources;
using Rhisis.World.Game.Entities;
using System.Collections.Generic;

namespace Rhisis.World.Game.Maps.Regions
{
    public class MapRespawnRegion : MapRegion, IMapRespawnRegion
    {
        public WorldObjectType ObjectType { get; private set; }

        public int ModelId { get; private set; }

        public int Time { get; private set; }

        public int Count { get; private set; }

        public IList<IWorldEntity> Entities { get; private set; }

        public MapRespawnRegion(int x, int z, int width, int length, int time, WorldObjectType type, int modelId, int count) 
            : base(x, z, width, length)
        {
            this.ModelId = modelId;
            this.Time = time;
            this.ObjectType = type;
            this.Count = count;
            this.Entities = new List<IWorldEntity>();
        }

        public override object Clone()
        {
            var region = new MapRespawnRegion(this.X, this.Z, this.Width, this.Length, this.Time, this.ObjectType, this.ModelId, this.Count)
            {
                IsActive = this.IsActive
            };
            
            return region;
        }

        public static IMapRespawnRegion FromRgnElement(RgnRespawn7 region)
        {
            return new MapRespawnRegion(region.Left, region.Top, region.Right - region.Left, region.Bottom - region.Top, 
                region.Time, (WorldObjectType)region.Type, region.Model, region.Count);
        }
    }
}
