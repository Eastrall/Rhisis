using Rhisis.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace Rhisis.World.Game.Core
{
    public abstract class Context : IContext
    {
        protected static readonly object SyncRoot = new object();

        private readonly IDictionary<uint, IEntity> _entities;
        private bool _disposedValue;

        /// <inheritdoc />
        public double GameTime { get; protected set; }

        /// <inheritdoc />
        public IEnumerable<IEntity> Entities => this._entities.Values;

        /// <summary>
        /// Creates a new <see cref="Context"/> instance.
        /// </summary>
        public Context()
        {
            this._entities = new Dictionary<uint, IEntity>();
        }

        /// <inheritdoc />
        public TEntity CreateEntity<TEntity>() where TEntity : IEntity
        {
            var entity = Activator.CreateInstance(typeof(TEntity), this) as IEntity;

            if (entity == null)
                throw new RhisisException($"An error occured while creating an entity of type {typeof(TEntity)}");

            lock (SyncRoot)
            {
                if (!this._entities.TryAdd(entity.Id, entity))
                    throw new RhisisException($"An error occured while adding the entity to the context list.");
            }

            return (TEntity)entity;
        }

        /// <inheritdoc />
        public bool DeleteEntity(uint id)
        {
            bool removed = false;

            lock (SyncRoot)
            {
                removed = this._entities.Remove(id);
            }
            
            return removed;
        }

        /// <inheritdoc />
        public bool DeleteEntity(IEntity entity) => this.DeleteEntity(entity.Id);

        /// <inheritdoc />
        public virtual TEntity FindEntity<TEntity>(uint id) where TEntity : IEntity 
            => this._entities.TryGetValue(id, out IEntity entity) ? (TEntity)entity : default(TEntity);

        /// <inheritdoc />
        public abstract void Update();

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the context resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    foreach (var entity in this._entities)
                        entity.Value.Dispose();

                    this._entities.Clear();
                }

                this._disposedValue = true;
            }
        }
    }
}