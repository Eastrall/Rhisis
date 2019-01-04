using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core.Systems;
using System;
using System.Collections.Generic;

namespace Rhisis.World.Game.Core
{
    /// <summary>
    /// Describes the Entity behavior.
    /// </summary>
    public interface IEntity : IDisposable, IEqualityComparer<IEntity>
    {
        /// <summary>
        /// Gets the entity id.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Gets the entity context.
        /// </summary>
        IContext Context { get; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        WorldEntityType Type { get; }

        /// <summary>
        /// Gets the object component of this entity.
        /// </summary>
        ObjectComponent Object { get; set; }

        /// <summary>
        /// Notifies and executes a system.
        /// </summary>
        /// <typeparam name="TSystem">System type</typeparam>
        /// <param name="e">System event arguments</param>
        void NotifySystem<TSystem>(SystemEventArgs e = null) where TSystem : ISystem;

        /// <summary>
        /// Finds an entity in the spawn list of the current entity.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="id">Entity id</param>
        /// <returns>Entity</returns>
        TEntity FindEntity<TEntity>(uint id) where TEntity : IEntity;
    }
}