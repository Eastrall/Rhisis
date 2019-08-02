using Rhisis.World.Game.Entities;
using System.Collections.Generic;

namespace Rhisis.World.Game.Maps
{
    /// <summary>
    /// Provides a mechanism to manage a map context.
    /// </summary>
    public interface IMapContext
    {
        /// <summary>
        /// Gets the map id.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets or sets the entities list.
        /// </summary>
        IReadOnlyDictionary<uint, IWorldEntity> Entities { get; }

        /// <summary>
        /// Adds a new entity to the context.
        /// </summary>
        /// <param name="entityToAdd">Entity to add.</param>
        void AddEntity(IWorldEntity entityToAdd);

        /// <summary>
        /// Deletes an entity from the current context.
        /// </summary>
        /// <param name="entityToDelete">Entity to delete.</param>
        void DeleteEntity(IWorldEntity entityToDelete);

        /// <summary>
        /// Gets an entity by its id.
        /// </summary>
        /// <typeparam name="TEntity">Entity type that inherits from <see cref="IWorldEntity"/>.</typeparam>
        /// <param name="id">Entity id.</param>
        /// <returns>Matching <typeparamref name="TEntity"/>; null otherwise.</returns>
        TEntity GetEntity<TEntity>(uint id) where TEntity : IWorldEntity;
    }
}
