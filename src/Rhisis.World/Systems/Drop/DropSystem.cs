using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
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
        private const float DropCircleRadius = 0.1f;

        private static readonly ILogger<DropSystem> Logger = DependencyContainer.Instance.Resolve<ILogger<DropSystem>>();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Mover;

        /// <inheritdoc />
        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (args == null)
            {
                Logger.LogError($"Cannot execute {nameof(DropSystem)}. Arguments are null.");
                return;
            }

            if (!args.GetCheckArguments())
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

        /// <summary>
        /// Drops an item to the current entity layer.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="e">Drop item event</param>
        private void DropItem(IWorldEntity entity, DropItemEventArgs e)
        {
            var worldServerConfiguration = DependencyContainer.Instance.Resolve<WorldConfiguration>();
            var drop = new ItemEntity(); 

            drop.Drop.Item = new Item(e.Item.Id, e.Item.Quantity, -1, -1, -1, e.Item.Refine);

            if (e.Owner != null)
            {
                drop.Drop.Owner = e.Owner;
                drop.Drop.OwnershipTime = Time.TimeInSeconds() + worldServerConfiguration.Drops.OwnershipTime;
                drop.Drop.DespawnTime = Time.TimeInSeconds() + worldServerConfiguration.Drops.DespawnTime;
            }

            drop.Object = new ObjectComponent
            {
                CurrentMap = null, // TODO: set map correctly
                MapId = entity.Object.MapId,
                LayerId = entity.Object.LayerId,
                ModelId = e.Item.Id,
                Name = drop.Drop.Item.Data.Name,
                Spawned = true,
                Position = Vector3.GetRandomPositionInCircle(entity.Object.Position, DropCircleRadius),
                Type = WorldObjectType.Item
            };
        }

        /// <summary>
        /// Drops gold to the current entity layer.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="e">Drop gold event</param>
        private void DropGold(IWorldEntity entity, DropGoldEventArgs e)
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

            this.DropItem(entity, new DropItemEventArgs(new Item(goldItemId, goldAmount), e.Owner));
        }
    }
}
