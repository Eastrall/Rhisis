using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Maps;
using Rhisis.World.Game.Maps.Regions;
using System;

namespace Rhisis.World.Game.Factories.Internal
{
    [Injectable(ServiceLifetime.Singleton)]
    public sealed class MonsterFactory : IMonsterFactory
    {
        private readonly IGameResources _gameResources;
        private readonly IBehaviorManager _behaviorManager;

        public MonsterFactory(IGameResources gameResources, IBehaviorManager behaviorManager)
        {
            this._gameResources = gameResources;
            this._behaviorManager = behaviorManager;
        }

        /// <inheritdoc />
        public IMonsterEntity CreateMonster(IMapInstance currentMap, IMapLayer currentMapLayer, int moverId, IMapRespawnRegion region)
        {
            if (!this._gameResources.Movers.TryGetValue(moverId, out MoverData moverData))
            {
                throw new ArgumentException($"Cannot find mover with id '{moverId}' in game resources.", nameof(moverId));
            }

            var monster = new MonsterEntity
            {
                Object = new ObjectComponent
                {
                    ModelId = moverId,
                    Name = moverData.Name,
                    Level = moverData.Level,
                    CurrentMap = currentMap,
                    LayerId = currentMapLayer.Id
                },
                Health = new HealthComponent
                {
                    Hp = moverData.AddHp,
                    Mp = moverData.AddMp,
                    Fp = 0
                }
            };

            monster.Moves = new MovableComponent
            {
                Speed = moverData.Speed / 2,
            };
            monster.Attributes.ResetAttribute(DefineAttributes.STR, moverData.Strength);
            monster.Attributes.ResetAttribute(DefineAttributes.STA, moverData.Stamina);
            monster.Attributes.ResetAttribute(DefineAttributes.DEX, moverData.Dexterity);
            monster.Attributes.ResetAttribute(DefineAttributes.INT, moverData.Intelligence);
            monster.Data = moverData;
            monster.Region = region;

            if (monster.Data.Class == MoverClassType.RANK_BOSS)
                monster.Object.Size *= 2;

            return monster;
        }
        public IMonsterEntity DuplicateMonster(IMonsterEntity baseMonster, Vector3 position, bool respawn = false)
        {           
            var monsterToSpawn = new MonsterEntity
            {
                Object = new ObjectComponent
                {
                    ModelId = baseMonster.Object.ModelId,
                    Type = WorldObjectType.Mover,
                    Size = baseMonster.Object.Size,
                    MovingFlags = ObjectState.OBJSTA_STAND,
                    Spawned = true,
                    AbleRespawn = respawn,
                    CurrentMap = baseMonster.Object.CurrentMap,
                    LayerId = baseMonster.Object.LayerId,
                    Position = position,
                    Angle = RandomHelper.FloatRandom(0, 360f),
                },
                Timers = new TimerComponent
                {
                    NextMoveTime = RandomHelper.LongRandom(8, 20)
                },
                Health = new HealthComponent
                {
                    Hp = baseMonster.Health.Hp,
                    Mp = baseMonster.Health.Mp,
                    Fp = 0
                }
            };
            monsterToSpawn.Moves = new MovableComponent
            {
                Speed = baseMonster.Moves.Speed / 2,
                DestinationPosition = monsterToSpawn.Object.Position.Clone()
            };

            monsterToSpawn.Data = baseMonster.Data;
            monsterToSpawn.Attributes = baseMonster.Attributes;
            monsterToSpawn.Region = baseMonster.Region;
            monsterToSpawn.Behavior = this._behaviorManager.GetBehavior(BehaviorType.Monster, monsterToSpawn, monsterToSpawn.Object.ModelId);

            return monsterToSpawn;
        }
    }
}
