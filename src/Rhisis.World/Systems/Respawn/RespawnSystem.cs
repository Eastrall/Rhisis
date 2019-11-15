using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems
{
    [Injectable]
    public sealed class RespawnSystem : IRespawnSystem
    {
        private readonly ILogger<RespawnSystem> _logger;

        /// <summary>
        /// Creates a new <see cref="RespawnSystem"/> instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public RespawnSystem(ILogger<RespawnSystem> logger)
        {
            this._logger = logger;
        }

        /// <inheritdoc />
        public void Execute(IWorldEntity entity)
        {
            if (entity is IMonsterEntity monster && monster.Health.IsDead)
            {
                if (monster.Object.Spawned && monster.Timers.DespawnTime < Time.TimeInSeconds())
                {
                    this._logger.LogDebug($"Despawning {monster.Object.Name}...");
                    monster.Object.Spawned = false;
                    if (monster.Object.IsSummoned != true) {
                        monster.Timers.RespawnTime = Time.TimeInSeconds() + monster.Region.Time;
                    } 
                }
                else if (!monster.Object.Spawned && monster.Timers.RespawnTime < Time.TimeInSeconds() && monster.Object.IsSummoned != true )
                {
                    this._logger.LogDebug($"Respawning {monster.Object.Name}...");
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

        /// <summary>
        /// Resets a monster.
        /// </summary>
        /// <param name="monster"></param>
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

        /// <summary>
        /// Resets a drop ownership.
        /// </summary>
        /// <param name="dropItem"></param>
        private void ResetDropOwnership(IItemEntity dropItem)
        {
            dropItem.Drop.Owner = null;
            dropItem.Drop.OwnershipTime = 0;
        }
    }
}
