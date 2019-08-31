using Rhisis.Core.Common;
using Rhisis.World.Game.Entities;
using System.Collections.Generic;

namespace Rhisis.World.Game.Maps.Regions
{
    public interface IMapRespawnRegion : IMapRegion
    {
        WorldObjectType ObjectType { get; }

        int ModelId { get; }

        int Time { get; }

        int Count { get; }

        IList<IWorldEntity> Entities { get; }
    }
}
