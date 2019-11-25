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
        public IMonsterEntity CreateMonster(IMapInstance currentMap, IMapLayer currentMapLayer, int moverId, IMapRespawnRegion region, bool respawn = false)
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
                    Type = WorldObjectType.Mover,
                    Size = ObjectComponent.DefaultObjectSize,
                    MovingFlags = ObjectState.OBJSTA_STAND,
                    Spawned = true,
                    AbleRespawn = respawn,
                    Name = moverData.Name,
                    Level = moverData.Level,
                    CurrentMap = currentMap,
                    LayerId = currentMapLayer.Id
                },
                Timers = new TimerComponent
                {
                    NextMoveTime = RandomHelper.LongRandom(8, 20)
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
            monster.Behavior = this._behaviorManager.GetBehavior(BehaviorType.Monster, monster, monster.Object.ModelId);

            if (monster.Data.Class == MoverClassType.RANK_BOSS)
                monster.Object.Size *= 2;

            return monster;
        }
        public IMonsterEntity DuplicateMonster(IMonsterEntity baseMonster, Vector3 position)
        {           
            var monsterToSpawn = new MonsterEntity();

            monsterToSpawn.Object = baseMonster.Object;
            monsterToSpawn.Timers = baseMonster.Timers;
            monsterToSpawn.Health = baseMonster.Health;
            monsterToSpawn.Moves = baseMonster.Moves;
            monsterToSpawn.Attributes = baseMonster.Attributes;
            monsterToSpawn.Behavior = baseMonster.Behavior;
            monsterToSpawn.Data   = baseMonster.Data;
            monsterToSpawn.Region = baseMonster.Region;

            monsterToSpawn.Object.Position = position;
            monsterToSpawn.Moves.DestinationPosition = position;
            monsterToSpawn.Object.Angle = RandomHelper.FloatRandom(0, 360f);

            return monsterToSpawn;
        }
    }
}
