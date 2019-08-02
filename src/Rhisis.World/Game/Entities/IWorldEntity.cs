using Rhisis.World.Game.Common;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;
using System;
using System.Collections.Generic;

namespace Rhisis.World.Game.Entities
{
    /// <summary>
    /// Describes how a world entity.
    /// </summary>
    public interface IWorldEntity : IDisposable, IEqualityComparer<IWorldEntity>, IEquatable<IWorldEntity>
    {
        /// <summary>
        /// Gets the entity id.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        WorldEntityType Type { get; }

        /// <summary>
        /// Gets the object component of this entity.
        /// </summary>
        ObjectComponent Object { get; set; }

        /// <summary>
        /// Gets the entity action delayer.
        /// </summary>
        Delayer Delayer { get; }

        /// <summary>
        /// Finds an entity in the spawn list of the current entity.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="id">Entity id</param>
        /// <returns>Entity</returns>
        TEntity FindEntity<TEntity>(uint id) where TEntity : IWorldEntity;

        /// <summary>
        /// Delete this entity from the current map.
        /// </summary>
        void Delete();
    }
}
