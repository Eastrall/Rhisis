using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
using Rhisis.Core.Structures;
using Rhisis.Core.Structures.Configuration;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Systems.Drop.EventArgs;

namespace Rhisis.World.Systems.Drop
{
    [System(SystemType.Notifiable)]
    public class DropSystem : ISystem
    {
        public const long MaxDropChance = 3000000000;
        private const int DropGoldLimit1 = 9;
        private const int DropGoldLimit2 = 49;
        private const int DropGoldLimit3 = 99;

        private static readonly ILogger<DropSystem> Logger = DependencyContainer.Instance.Resolve<ILogger<DropSystem>>();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Mover;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs args)
        {
            if (args == null)
            {
                Logger.LogError($"Cannot execute {nameof(DropSystem)}. Arguments are null.");
                return;
            }

            if (!args.CheckArguments())
            {
                Logger.LogError($"Cannot execute {nameof(DropSystem)} action: {args.GetType()} due to invalid arguments.");
                return;
            }

            switch (args)
            {
                case DropItemEventArgs dropItemEventArgs:
                    this.DropItem(entity, dropItemEventArgs);
                    break;
                case DropGoldEventArgs dropGoldEventArgs:
                    this.DropGold(entity, dropGoldEventArgs);
                    break;
            }
        }

        private void DropItem(IEntity entity, DropItemEventArgs e)
        {
            int itemRefine = RandomHelper.Random(0, e.Item.ItemMaxRefine);
            var worldServerConfiguration = DependencyContainer.Instance.Resolve<WorldConfiguration>();
            var drop = entity.Object.CurrentLayer.CreateEntity<ItemEntity>();

            drop.Drop.Item = new Item(e.Item.ItemId, 1, -1, -1, -1, (byte)itemRefine);
            drop.Drop.Owner = e.Owner;
            drop.Drop.OwnershipTime = Time.TimeInSeconds() + worldServerConfiguration.Drops.OwnershipTime;
            drop.Drop.DespawnTime = Time.TimeInSeconds() + worldServerConfiguration.Drops.DespawnTime;
            drop.Object = new ObjectComponent
            {
                MapId = entity.Object.MapId,
                LayerId = entity.Object.LayerId,
                ModelId = e.Item.ItemId,
                Spawned = true,
                Position = Vector3.GetRandomPositionInCircle(entity.Object.Position, 0.5f),
                Type = WorldObjectType.Item
            };
        }

        private void DropGold(IEntity entity, DropGoldEventArgs e)
        {
            int goldAmount = e.GoldAmount;
            int goldItemId = DefineItem.II_GOLD_SEED1;
            var worldServerConfiguration = DependencyContainer.Instance.Resolve<WorldConfiguration>();

            goldAmount *= worldServerConfiguration.Rates.Gold;

            if (goldAmount <= 0)
                return;

            if (goldAmount > (DropGoldLimit1 * worldServerConfiguration.Rates.Gold))
                goldItemId = DefineItem.II_GOLD_SEED2;
            else if (goldAmount > (DropGoldLimit2 * worldServerConfiguration.Rates.Gold))
                goldItemId = DefineItem.II_GOLD_SEED3;
            else if (goldAmount > (DropGoldLimit3 * worldServerConfiguration.Rates.Gold))
                goldItemId = DefineItem.II_GOLD_SEED4;

            var drop = entity.Object.CurrentLayer.CreateEntity<ItemEntity>();

            drop.Drop.Item = new Item(goldItemId, goldAmount, (int)entity.Id);
            drop.Drop.Owner = e.Owner;
            drop.Drop.OwnershipTime = Time.TimeInSeconds() + worldServerConfiguration.Drops.OwnershipTime;
            drop.Drop.DespawnTime = Time.TimeInSeconds() + worldServerConfiguration.Drops.DespawnTime;
            drop.Object = new ObjectComponent
            {
                MapId = entity.Object.MapId,
                LayerId = entity.Object.LayerId,
                ModelId = goldItemId,
                Spawned = true,
                Position = Vector3.GetRandomPositionInCircle(entity.Object.Position, 0.5f),
                Type = WorldObjectType.Item
            };
        }
    }
}
