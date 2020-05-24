using Microsoft.Extensions.Options;
using Rhisis.Core.Data;
using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.Core.Structures.Configuration.World;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Battle;
using Rhisis.World.Systems.Drop;
using Rhisis.World.Systems.Follow;
using Rhisis.World.Systems.Mobility;
using System.Linq;

namespace Rhisis.World.Game.Behaviors
{
    /// <summary>
    /// Default behavior for all monsters.
    /// </summary>
    [Behavior(BehaviorType.Monster, isDefault: true)]
    public class DefaultMonsterBehavior : IBehavior
    {
        private const float MovingRange = 40f;

        private readonly IMonsterEntity _monster;
        private readonly WorldConfiguration _worldConfiguration;
        private readonly IGameResources _gameResources;
        private readonly IMobilitySystem _mobilitySystem;
        private readonly IBattleSystem _battleSystem;
        private readonly IFollowSystem _followSystem;
        private readonly IDropSystem _dropSystem;
        private readonly IMoverPacketFactory _moverPacketFactory;

        public DefaultMonsterBehavior(IMonsterEntity monster,
            IOptions<WorldConfiguration> worldConfiguration,
            IGameResources gameResources, 
            IMobilitySystem mobilitySystem, 
            IBattleSystem battleSystem, 
            IFollowSystem followSystem, 
            IDropSystem dropSystem, 
            IMoverPacketFactory moverPacketFactory)
        {
            _monster = monster;
            _worldConfiguration = worldConfiguration.Value;
            _gameResources = gameResources;
            _mobilitySystem = mobilitySystem;
            _battleSystem = battleSystem;
            _followSystem = followSystem;
            _dropSystem = dropSystem;
            _moverPacketFactory = moverPacketFactory;
        }

        /// <inheritdoc />
        public virtual void Update()
        {
            if (!_monster.Object.Spawned || _monster.IsDead)
            {
                return;
            }

            if (_monster.Battle.IsFighting)
            {
                ProcessMonsterFight(_monster);
            }
            else
            {
                ProcessMonsterMovements(_monster);
            }

            _mobilitySystem.CalculatePosition(_monster);
        }

        /// <inheritdoc />
        public virtual void OnArrived()
        {
            if (!_monster.Battle.IsFighting)
            {
                _monster.Timers.NextMoveTime = Time.TimeInSeconds() + RandomHelper.LongRandom(5, 10);
            }
        }

        /// <inheritdoc />
        public void OnTargetKilled(ILivingEntity killedEntity)
        {
            _monster.Battle.Reset();
            _monster.Follow.Reset();
        }

        /// <inheritdoc />
        public void OnKilled(ILivingEntity killerEntity)
        {
            _monster.Timers.DespawnTime = Time.TimeInSeconds() + 5; // TODO: Configure this timer on world configuration

            // Drop items
            int itemCount = 0;
            foreach (DropItemData dropItem in _monster.Data.DropItems)
            {
                if (itemCount >= _monster.Data.MaxDropItem)
                    break;

                long dropChance = RandomHelper.LongRandom(0, DropSystem.MaxDropChance);

                if (dropItem.Probability * _worldConfiguration.Rates.Drop >= dropChance)
                {
                    var item = new InventoryItem(dropItem.ItemId, 1, -1, -1, -1, (byte)RandomHelper.Random(0, dropItem.ItemMaxRefine));

                    _dropSystem.DropItem(_monster, item, killerEntity);
                    itemCount++;
                }
            }

            // Drop item kinds
            foreach (DropItemKindData dropItemKind in _monster.Data.DropItemsKind)
            {
                var itemsDataByItemKind = _gameResources.Items.Values.Where(x => x.ItemKind3 == dropItemKind.ItemKind && x.Rare >= dropItemKind.UniqueMin && x.Rare <= dropItemKind.UniqueMax);

                if (!itemsDataByItemKind.Any())
                {
                    continue;
                }

                var itemData = itemsDataByItemKind.ElementAt(RandomHelper.Random(0, itemsDataByItemKind.Count() - 1));

                int itemRefine = RandomHelper.Random(0, 10);

                for (int i = itemRefine; i >= 0; i--)
                {
                    long itemDropProbability = (long)(_gameResources.ExpTables.GetDropLuck(itemData.Level > 120 ? 119 : itemData.Level, itemRefine) * (_monster.Data.CorrectionValue / 100f));
                    long dropChance = RandomHelper.LongRandom(0, DropSystem.MaxDropChance);

                    if (dropChance < itemDropProbability * _worldConfiguration.Rates.Drop)
                    {
                        var item = new InventoryItem(itemData.Id, 1, -1, -1, -1, (byte)itemRefine);

                        _dropSystem.DropItem(_monster, item, killerEntity);
                        break;
                    }
                }
            }

            // Drop gold
            int goldDropped = RandomHelper.Random(_monster.Data.DropGoldMin, _monster.Data.DropGoldMax);
            _dropSystem.DropGold(_monster, goldDropped, killerEntity);
        }

        /// <summary>
        /// Update monster moves.
        /// </summary>
        /// <param name="monster"></param>
        private void ProcessMonsterMovements(IMonsterEntity monster)
        {
            if (monster.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_STAND))
            {
                if (monster.Timers.NextMoveTime < Time.TimeInSeconds())
                {
                    monster.Object.MovingFlags &= ~ObjectState.OBJSTA_STAND;
                    monster.Object.MovingFlags |= ObjectState.OBJSTA_FMOVE;
                    monster.Moves.DestinationPosition.Copy(monster.Region.GetRandomPosition());
                    monster.Object.Angle = Vector3.AngleBetween(monster.Object.Position, monster.Moves.DestinationPosition);

                    _moverPacketFactory.SendDestinationPosition(monster);
                    _moverPacketFactory.SendDestinationAngle(monster, false);
                }
                else if (monster.Moves.ReturningToOriginalPosition)
                {
                    monster.Attributes[DefineAttributes.HP] = monster.Data.AddHp;
                    _moverPacketFactory.SendUpdateAttributes(monster, DefineAttributes.HP, monster.Attributes[DefineAttributes.HP]);
                    monster.Moves.ReturningToOriginalPosition = false;
                }
                else if (monster.Moves.SpeedFactor >= 2f)
                {
                    monster.Moves.SpeedFactor = 1f;
                    _moverPacketFactory.SendSpeedFactor(monster, monster.Moves.SpeedFactor);
                }
            }
        }

        /// <summary>
        /// Process the monster's fight.
        /// </summary>
        /// <param name="monster"></param>
        private void ProcessMonsterFight(IMonsterEntity monster)
        {
            if (monster.Battle.Target.IsDead)
            {
                monster.Follow.Reset();
                monster.Battle.Reset();
                return;
            }

            if (monster.Follow.IsFollowing)
            {
                if (monster.Moves.SpeedFactor != 2f)
                {
                    monster.Moves.SpeedFactor = 2;
                    _moverPacketFactory.SendSpeedFactor(monster, monster.Moves.SpeedFactor);
                }

                if (monster.Object.Position.IsInCircle(monster.Follow.Target.Object.Position, monster.Follow.FollowDistance))
                {
                    if (monster.Timers.NextAttackTime <= Time.TimeInMilliseconds())
                    {
                        _battleSystem.MeleeAttack(monster, monster.Battle.Target, ObjectMessageType.OBJMSG_ATK1, monster.Data.AttackSpeed);
                        monster.Timers.NextAttackTime = (long)(Time.TimeInMilliseconds() + monster.Data.ReAttackDelay);
                    }
                }
                else
                {
                    _followSystem.Follow(monster, monster.Battle.Target);
                }
            }
            else
            {
                _followSystem.Follow(monster, monster.Battle.Target);
            }
        }
    }
}
