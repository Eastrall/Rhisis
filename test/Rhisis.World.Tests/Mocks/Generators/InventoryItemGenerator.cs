using Bogus;
using Rhisis.Database.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Tests.Mocks.Database.Entities
{
    internal class InventoryItemGenerator : Faker<DbInventoryItem>
    {
        public InventoryItemGenerator(int playerId, IEnumerable<DbItem> dbItems)
        {
            RuleFor(x => x.CharacterId, playerId)
                .RuleFor(x => x.Item, (f, p) => f.PickRandom(dbItems.Where(x => !x.IsDeleted)))
                .RuleFor(x => x.Quantity, (f, p) => f.Random.Int(0, 999))
                .RuleFor(x => x.Slot, (f, p) => f.IndexFaker)
                .FinishWith((faker, instance) =>
                {
                    instance.ItemId = instance.Item.Id;
                });
        }
    }
}
