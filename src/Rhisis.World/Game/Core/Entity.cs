using Rhisis.Core.Helpers;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Core
{
    /// <summary>
    /// Describes the Entity implementation.
    /// </summary>
    public abstract class Entity : IEntity
    {
        private bool _disposedValue;

        /// <inheritdoc />
        public uint Id { get; }

        /// <inheritdoc />
        public IContext Context { get; }

        /// <inheritdoc />
        public abstract WorldEntityType Type { get; }

        /// <inheritdoc />
        public ObjectComponent Object { get; set; }

        /// <summary>
        /// Creates a new <see cref="Entity"/> instance.
        /// </summary>
        /// <param name="context"></param>
        protected Entity(IContext context)
        {
            this.Id = RandomHelper.GenerateUniqueId();
            this.Context = context;
            this.Object = new ObjectComponent();
        }

        /// <inheritdoc />
        public void NotifySystem<TSystem>(SystemEventArgs e = null)
            where TSystem : ISystem => SystemManager.Instance.Execute<TSystem>(this, e);

        /// <inheritdoc />
        public TEntity FindEntity<TEntity>(uint id)
            where TEntity : IEntity => (TEntity)this.Object.Entities.FirstOrDefault(x => x is TEntity && x.Id == id);

        /// <inheritdoc />
        public bool Equals(IEntity x, IEntity y) => x.Id == y.Id;

        /// <inheritdoc />
        public int GetHashCode(IEntity obj)
        {
            var hashCode = 181846194; //TODO: implement constant and explain this number.

            hashCode *= -1521134295 + EqualityComparer<IEntity>.Default.GetHashCode(this); //TODO: implement constant and explain this number.

            return hashCode;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose inner resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    // Dispose resources
                }

                this._disposedValue = true;
            }
        }
    }
}