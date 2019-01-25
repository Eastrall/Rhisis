using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Core;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Maps.Regions
{
    public class MapRespawnRegion : MapRegion, IMapRespawnRegion
    {
        private string _name;

        public WorldObjectType ObjectType { get; }

        public int ModelId { get; }

        public int Time { get; }

        public int Count { get; }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this._name))
                    this._name = this.Entities.Any() ? this.Entities.FirstOrDefault().Object.Name : "Unknown";

                return this._name;
            }
        }

        public IList<IEntity> Entities { get; private set; }

        public MapRespawnRegion(int x, int z, int width, int length, int time, WorldObjectType type, int modelId, int count) 
            : base(x, z, width, length)
        {
            this.ModelId = modelId;
            this.Time = time;
            this.ObjectType = type;
            this.Count = count;
            this.Entities = new List<IEntity>();
        }

        public override object Clone()
        {
            var region = new MapRespawnRegion(this.X, this.Z, this.Width, this.Length, this.Time, this.ObjectType, this.ModelId, this.Count)
            {
                IsActive = this.IsActive
            };
            
            return region;
        }

        public override string ToString() => $"{this.Count} {this.Name}";

        public static IMapRespawnRegion FromRgnElement(RgnRespawn7 rgnRespawn)
        {
            return new MapRespawnRegion(rgnRespawn.Left, rgnRespawn.Top, rgnRespawn.Width, rgnRespawn.Length, 
                rgnRespawn.Time, (WorldObjectType)rgnRespawn.Type, rgnRespawn.Model, rgnRespawn.Count);
        }
    }
}
