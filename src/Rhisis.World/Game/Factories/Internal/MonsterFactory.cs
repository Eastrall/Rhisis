using Microsoft.Extensions.DependencyInjection;
using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Entities.Internal;
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
        private readonly IItemFactory _itemFactory;

        public MonsterFactory(IGameResources gameResources, IBehaviorManager behaviorManager, IItemFactory itemFactory)
        {
            _gameResources = gameResources;
            _behaviorManager = behaviorManager;
            _itemFactory = itemFactory;
        }

        /// <inheritdoc />
        public IMonsterEntity CreateMonster(IMapInstance currentMap, IMapLayer currentMapLayer, int moverId, IMapRespawnRegion region, bool respawn = false)
        {
            if (!_gameResources.Movers.TryGetValue(moverId, out MoverData moverData))
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
                    Type = WorldObjectType.Mover,
                    Size = ObjectComponent.DefaultObjectSize,
                    MovingFlags = ObjectState.OBJSTA_STAND,
                    Position = region.GetRandomPosition(),
                    Angle = RandomHelper.FloatRandom(0, 360f),
                    Spawned = true,
                    AbleRespawn = respawn,
                    CurrentMap = currentMap,
                    LayerId = currentMapLayer.Id
                },
                Timers = new TimerComponent
                {
                    NextMoveTime = Time.TimeInSeconds() + RandomHelper.LongRandom(8, 20)
                }
            };

            monster.Moves = new MovableComponent
            {
                Speed = moverData.Speed
            };

            monster.Data = moverData;
            monster.Region = region;
            monster.Attributes[DefineAttributes.HP] = moverData.AddHp;
            monster.Attributes[DefineAttributes.MP] = moverData.AddMp;
            monster.Attributes[DefineAttributes.STR] = moverData.Strength;
            monster.Attributes[DefineAttributes.STA] = moverData.Stamina;
            monster.Attributes[DefineAttributes.DEX] = moverData.Dexterity;
            monster.Attributes[DefineAttributes.INT] = moverData.Intelligence;
            monster.Behavior = _behaviorManager.GetBehavior(BehaviorType.Monster, monster, moverId);
            monster.Hand = _itemFactory.CreateInventoryItem(11, 0, ElementType.None, 0);

            if (moverData.Class == MoverClassType.RANK_BOSS)
            {
                monster.Object.Size *= 2;
            }

            return monster;
        }
    }
}
