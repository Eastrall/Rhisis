using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Loaders;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.World.Game.Structures;
using System.Collections.Generic;
using System.Linq;
using Rhisis.Core.Data;
using Rhisis.World.Packets;
using Rhisis.World.Game.Behaviors;

namespace Rhisis.World.Game.Maps
{
    public class MapLayer : Context, IMapLayer
    {
        private readonly List<IMapRegion> _regions;
        private readonly IGameResources _gameResources;
        private readonly IBehaviorManager _behaviorManager;

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public IMapInstance Parent { get; }

        /// <inheritdoc />
        public ICollection<IMapRegion> Regions => this._regions;

        /// <summary>
        /// Creates a new <see cref="MapLayer"/> instance.
        /// </summary>
        /// <param name="parent">Parent map instance</param>
        /// <param name="id">Map layer id</param>
        public MapLayer(IMapInstance parent, int id)
        {
            this.Id = id;
            this.Parent = parent;
            this._regions = new List<IMapRegion>();

            foreach (IMapRegion region in this.Parent.Regions)
            {
                if (region is IMapRespawnRegion respawner)
                {
                    var respawnerRegion = respawner.Clone() as IMapRespawnRegion;

                    if (respawnerRegion.ObjectType == WorldObjectType.Mover)
                    {
                        var moverData = this._gameResources.Movers[respawnerRegion.ModelId];
                        for (var i = 0; i < respawnerRegion.Count; i++)
                            respawnerRegion.Entities.Add(this.CreateMonster(moverData, respawnerRegion));
                    }
                    else if (respawnerRegion.ObjectType == WorldObjectType.Item)
                    {
                        var itemData = this._gameResources.Items[respawnerRegion.ModelId];
                        for (var i = 0; i < respawnerRegion.Count; i++)
                            respawnerRegion.Entities.Add(this.CreateWorldItem(itemData, respawnerRegion));
                    }

                    this._regions.Add(respawnerRegion);
                }
                else
                    this._regions.Add(region);
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            for (int i = 0; i < this.Entities.Count(); i++)
                SystemManager.Instance.ExecuteUpdatable(this.Entities.ElementAt(i));

            for (int i = 0; i < this._regions.Count; i++)
            {
                if (this._regions[i].IsActive && this._regions[i] is IMapRespawnRegion respawnRegion)
                {
                    for (int j = 0; j < respawnRegion.Entities.Count; j++)
                    {
                        SystemManager.Instance.ExecuteUpdatable(respawnRegion.Entities[j]);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void UpdateDeletedEntities()
        {
            while (this._entitiesToDelete.TryDequeue(out uint entityIdToDelete))
            {
                var entityToDelete = this.FindEntity<IEntity>(entityIdToDelete);

                if (entityToDelete != null)
                {
                    foreach (IEntity entity in entityToDelete.Object.Entities)
                    {
                        if (entity.Type == WorldEntityType.Player)
                            WorldPacketFactory.SendDespawnObjectTo(entity as IPlayerEntity, entityToDelete);

                        entity.Object.Entities.Remove(entityToDelete);
                    }

                    this._entities.Remove(entityIdToDelete);
                }
            }
        }

        /// <inheritdoc />
        public override TEntity FindEntity<TEntity>(uint id)
        {
            var entity = base.FindEntity<TEntity>(id);

            if (entity == null)
            {
                for (int i = 0; i < this._regions.Count; i++)
                {
                    if (this._regions[i].IsActive && this._regions[i] is IMapRespawnRegion respawnRegion)
                    {
                        var regionEntity = respawnRegion.Entities.FirstOrDefault(x => x.Id == id);

                        if (regionEntity != null)
                        {
                            entity = (TEntity)regionEntity;
                            break;
                        }
                    }
                }
            }

            return entity;
        }

        /// <summary>
        /// Create a new monster.
        /// </summary>
        /// <param name="moverData">Monster's data</param>
        /// <param name="respawnRegion">Respawn region</param>
        /// <returns></returns>
        private MonsterEntity CreateMonster(MoverData moverData, IMapRespawnRegion respawnRegion)
        {
            var monster = new MonsterEntity(this);

            monster.Object = new ObjectComponent
            {
                MapId = this.Parent.Id,
                LayerId = this.Id,
                ModelId = moverData.Id,
                Type = WorldObjectType.Mover,
                Position = respawnRegion.GetRandomPosition(),
                Angle = RandomHelper.FloatRandom(0, 360f),
                Name = moverData.Name,
                Size = ObjectComponent.DefaultObjectSize,
                Spawned = true,
                Level = moverData.Level,
                MovingFlags = ObjectState.OBJSTA_STAND
            };
            monster.Timers = new TimerComponent
            {
                NextMoveTime = RandomHelper.LongRandom(8, 20)
            };
            monster.Moves = new MovableComponent
            {
                Speed = moverData.Speed / 2,
                DestinationPosition = monster.Object.Position.Clone()
            };
            monster.Health = new HealthComponent
            {
                Hp = moverData.AddHp,
                Mp = moverData.AddMp,
                Fp = 0
            };
            monster.Attributes.ResetAttribute(DefineAttributes.STR, moverData.Strength);
            monster.Attributes.ResetAttribute(DefineAttributes.STA, moverData.Stamina);
            monster.Attributes.ResetAttribute(DefineAttributes.DEX, moverData.Dexterity);
            monster.Attributes.ResetAttribute(DefineAttributes.INT, moverData.Intelligence);
            monster.Behavior = this._behaviorManager.GetBehavior(BehaviorType.Monster, monster, monster.Object.ModelId);
            monster.Region = respawnRegion;
            monster.Data = moverData;

            if (moverData.Class == MoverClassType.RANK_BOSS)
                monster.Object.Size *= 2;

            return monster;
        }

        /// <summary>
        /// Creates a permanant drop item.
        /// </summary>
        /// <remarks>
        /// Mainly used for quest items.
        /// </remarks>
        /// <param name="itemData">Item data</param>
        /// <param name="respawnRegion">Respawn region</param>
        /// <returns>A new ItemEntity</returns>
        private ItemEntity CreateWorldItem(ItemData itemData, IMapRespawnRegion respawnRegion)
        {
            var item = new ItemEntity(this);

            item.Drop.Item = new Item(itemData.Id, 1);
            item.Drop.RespawnTime = respawnRegion.Time;
            item.Region = respawnRegion;
            item.Object = new ObjectComponent
            {
                MapId = this.Parent.Id,
                LayerId = this.Id,
                ModelId = respawnRegion.ModelId,
                Type = WorldObjectType.Item,
                Position = respawnRegion.GetRandomPosition(),
                Angle = RandomHelper.FloatRandom(0, 360f),
                Name = itemData.Name,
                Size = ObjectComponent.DefaultObjectSize,
                Spawned = true,
            };

            return item;
        }
    }
}
