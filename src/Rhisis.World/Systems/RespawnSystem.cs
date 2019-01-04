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

        public void Execute(IEntity entity, SystemEventArgs args)
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

            if (entity is IItemEntity dropItem && dropItem.Drop.OwnershipTime <= Time.TimeInSeconds())
            {
                if (dropItem.Drop.HasOwner && dropItem.Drop.OwnershipTime <= Time.TimeInSeconds())
                {
                    this.ResetDropOwnership(dropItem);
                }

                // TODO: despawn drop
            }
        }

        private void ResetMonster(IMonsterEntity monster)
        {
            monster.Timers.NextMoveTime = Time.TimeInSeconds() + RandomHelper.LongRandom(5, 15);
            monster.Object.Spawned = true;
            monster.Object.Position = monster.Region.GetRandomPosition();
            monster.MovableComponent.DestinationPosition = monster.Object.Position.Clone();
            monster.MovableComponent.SpeedFactor = 1;
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
