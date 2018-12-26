using System;
using System.Collections.Generic;

namespace Rhisis.World.Game.Core
{
    /// <summary>
    /// Defines a context.
    /// </summary>
    public interface IContext : IDisposable
    {
        /// <summary>
        /// Gets the update time of the async context.
        /// </summary>
        double GameTime { get; }

        /// <summary>
        /// Gets the list of all entities of this context.
        /// </summary>
        IEnumerable<IEntity> Entities { get; }

        /// <summary>
        /// Creates a new entity in this context.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns></returns>
        TEntity CreateEntity<TEntity>() where TEntity : IEntity;

        /// <summary>
        /// Find an entity in this context.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="id">Entity id</param>
        /// <returns></returns>
        TEntity FindEntity<TEntity>(uint id) where TEntity : IEntity;

        /// <summary>
        /// Deletes the entity from this context.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="id">Entity id</param>
        /// <returns></returns>
        bool DeleteEntity(uint id);

        /// <summary>
        /// Deletes the entity from this context.
        /// </summary>
        /// <param name="entity">Entity instance</param>
        /// <returns></returns>
        bool DeleteEntity(IEntity entity);

        /// <summary>
        /// Update this context.
        /// </summary>
        void Update();
    }
}