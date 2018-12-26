using Rhisis.Core.Common;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.Resources;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Loaders;
using Rhisis.World.Game.Maps.Regions;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Maps
{
    public class MapLayer : Context, IMapLayer
    {
        private readonly IList<IMapRegion> _regions;

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
                if (region is IMapRespawnRegion respawner && respawner.ObjectType == WorldObjectType.Mover)
                {
                    var respawnerRegion = respawner.Clone() as IMapRespawnRegion;

                    for (var i = 0; i < respawnerRegion.Count; i++)
                        this.CreateMonster(respawner);

                    this._regions.Add(respawner);
                }
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            this.GameTime = this.Parent.GameTime;

            foreach (var entity in this.Entities)
                SystemManager.Instance.ExecuteUpdatable(entity);
            
            foreach (var region in this._regions)
            {
                if (region.IsActive && region is IMapRespawnRegion respawnRegion)
                {
                    foreach (var entity in respawnRegion.Entities)
                    {
                        SystemManager.Instance.ExecuteUpdatable(entity);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override TEntity FindEntity<TEntity>(uint id)
        {
            var entity = base.FindEntity<TEntity>(id);

            if (entity == null)
            {
                foreach (var region in this._regions)
                {
                    if (region.IsActive && region is IMapRespawnRegion respawnRegion)
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
        /// <param name="respawner"></param>
        private void CreateMonster(IMapRespawnRegion respawner)
        {
            var monster = new MonsterEntity(this);
            var moverData = GameResources.Instance.Movers[respawner.ModelId];
            var behaviors = DependencyContainer.Instance.Resolve<BehaviorLoader>();

            monster.Object = new ObjectComponent
            {
                MapId = this.Parent.Id,
                LayerId = this.Id,
                ModelId = moverData.Id,
                Type = WorldObjectType.Mover,
                Position = respawner.GetRandomPosition(),
                Angle = RandomHelper.FloatRandom(0, 360f),
                Name = moverData.Name,
                Size = ObjectComponent.DefaultObjectSize,
                Spawned = true,
                Level = moverData.Level
            };
            monster.Timers = new TimerComponent
            {
                NextMoveTime = RandomHelper.LongRandom(8, 20)
            };
            monster.MovableComponent = new MovableComponent
            {
                Speed = moverData.Speed,
                DestinationPosition = monster.Object.Position.Clone()
            };
            monster.Health = new HealthComponent
            {
                Hp = moverData.AddHp,
                Mp = moverData.AddMp,
                Fp = 0
            };
            monster.Statistics = new StatisticsComponent
            {
                Strength = (ushort)moverData.Strength,
                Stamina = (ushort)moverData.Stamina,
                Dexterity = (ushort)moverData.Dexterity,
                Intelligence = (ushort)moverData.Intelligence
            };
            monster.Behavior = behaviors.MonsterBehaviors.GetBehavior(monster.Object.ModelId);
            monster.Region = respawner;
            monster.Data = moverData;

            respawner.Entities.Add(monster);
        }
    }
}
