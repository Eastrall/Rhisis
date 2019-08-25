using Rhisis.Core.Helpers;
using Rhisis.World.Game.Common;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;
using System;
using System.Linq;

namespace Rhisis.World.Game.Entities
{
    public abstract class WorldEntity : IWorldEntity
    {
        private bool _disposedValue;

        /// <inheritdoc />
        public uint Id { get; }

        /// <inheritdoc />
        public abstract WorldEntityType Type { get; }

        /// <inheritdoc />
        public ObjectComponent Object { get; set; }

        /// <inheritdoc />
        public Delayer Delayer { get; }

        /// <summary>
        /// Creates a new <see cref="WorldEntity"/> instance.
        /// </summary>
        protected WorldEntity()
        {
            this.Id = RandomHelper.GenerateUniqueId();
            this.Object = new ObjectComponent();
            this.Delayer = new Delayer();
        }

        /// <inheritdoc />
        public TEntity FindEntity<TEntity>(uint id) where TEntity : IWorldEntity 
            => (TEntity)this.Object.Entities.FirstOrDefault(x => x is TEntity && x.Id == id);

        /// <inheritdoc />
        public TEntity FindEntity<TEntity>(Func<TEntity, bool> predicate) where TEntity : IWorldEntity
            => (TEntity)this.Object.Entities.FirstOrDefault(x => x is TEntity entity && predicate(entity));

        /// <inheritdoc />
        public void Delete()
        {
            this.Object.CurrentMap.DeleteEntity(this);
            this.Object.CurrentLayer.DeleteEntity(this);
        }

        /// <inheritdoc />
        public bool Equals(IWorldEntity x, IWorldEntity y) => x.Equals(y);

        /// <inheritdoc />
        public bool Equals(IWorldEntity other)
            => (this.Id, this.Type, this.Object.MapId, this.Object.LayerId) == (other.Id, other.Type, other.Object.MapId, other.Object.LayerId);

        /// <inheritdoc />
        public int GetHashCode(IWorldEntity obj) => (obj.Id, obj.Type, obj.Object.Name, obj.Object.Type).GetHashCode();

        /// <summary>
        /// Disposes the <see cref="WorldEntity"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    this.Delayer.Dispose();
                }

                this._disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose() => this.Dispose(true);
    }
}
