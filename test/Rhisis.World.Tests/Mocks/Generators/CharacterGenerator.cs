using Bogus;
using Rhisis.Database.Entities;

namespace Rhisis.World.Tests.Mocks.Database.Entities
{
    internal class CharacterGenerator : Faker<DbCharacter>
    {
        public CharacterGenerator()
        {
            RuleFor(x => x.Name, (faker, prop) => faker.Internet.UserName());
        }
    }
}
