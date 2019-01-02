using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
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
                    this.DropItem();
                    break;
                case DropGoldEventArgs dropGoldEventArgs:
                    this.DropGold(entity, dropGoldEventArgs.GoldAmount);
                    break;
            }
        }

        private void DropItem()
        {
            // TODO
        }

        private void DropGold(IEntity entity, int goldAmount)
        {
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
            drop.Object = new ObjectComponent
            {
                MapId = entity.Object.MapId,
                LayerId = entity.Object.LayerId,
                ModelId = goldItemId,
                Spawned = true,
                Position = Vector3.GetRandomPositionInCircle(entity.Object.Position, 0.6f),
                Type = WorldObjectType.Item
            };
        }
    }
}
