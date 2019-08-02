using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems
{
    [System]
    public sealed class RespawnSystem : ISystem
    {
        private static readonly ILogger<RespawnSystem> Logger = DependencyContainer.Instance.Resolve<ILogger<RespawnSystem>>();

        public WorldEntityType Type => WorldEntityType.Monster | WorldEntityType.Drop;

        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (entity is IMonsterEntity monster && monster.Health.IsDead)
            {
                if (monster.Object.Spawned && monster.Timers.DespawnTime < Time.TimeInSeconds())
                {
                    Logger.LogDebug($"Despawning {monster.Object.Name}...");
                    monster.Object.Spawned = false;
                    monster.Timers.RespawnTime = Time.TimeInSeconds() + monster.Region.Time;
                }
                else if (!monster.Object.Spawned && monster.Timers.RespawnTime < Time.TimeInSeconds())
                {
                    Logger.LogDebug($"Respawning {monster.Object.Name}...");
                    this.ResetMonster(monster);
                }
            }

            if (entity is IItemEntity dropItem)
            {
                if (dropItem.Drop.HasOwner && dropItem.Drop.OwnershipTime <= Time.TimeInSeconds())
                {
                    this.ResetDropOwnership(dropItem);
                }

                if (dropItem.Drop.IsTemporary && dropItem.Drop.DespawnTime <= Time.TimeInSeconds())
                {
                    this.ResetDropOwnership(dropItem);
                    dropItem.Object.Spawned = false;
                    dropItem.Delete();
                }

                if (!dropItem.Drop.IsTemporary && !dropItem.Object.Spawned && dropItem.Drop.RespawnTime <= Time.TimeInSeconds())
                {
                    dropItem.Object.Position = dropItem.Region.GetRandomPosition();
                    dropItem.Object.Spawned = true;
                }
            }
        }

        private void ResetMonster(IMonsterEntity monster)
        {
            monster.Timers.NextMoveTime = Time.TimeInSeconds() + RandomHelper.LongRandom(5, 15);
            monster.Object.Spawned = true;
            monster.Object.Position = monster.Region.GetRandomPosition();
            monster.Moves.DestinationPosition = monster.Object.Position.Clone();
            monster.Moves.SpeedFactor = 1;
            monster.Health.Hp = monster.Data.AddHp;
            monster.Health.Mp = monster.Data.AddMp;
        }

        private void ResetDropOwnership(IItemEntity dropItem)
        {
            dropItem.Drop.Owner = null;
            dropItem.Drop.OwnershipTime = 0;
        }
    }
}
