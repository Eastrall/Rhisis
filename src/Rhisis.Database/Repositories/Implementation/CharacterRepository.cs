using Microsoft.EntityFrameworkCore;
using Rhisis.Database.Entities;
using System.Linq;

namespace Rhisis.Database.Repositories.Implementation
{
    /// <summary>
    /// Character repository.
    /// </summary>
    internal sealed class CharacterRepository : RepositoryBase<DbCharacter>, ICharacterRepository
    {
        /// <summary>
        /// Creates and initialize the <see cref="CharacterRepository"/>.
        /// </summary>
        /// <param name="context"></param>
        public CharacterRepository(DbContext context) 
            : base(context)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<DbCharacter> GetQueryable(DbContext context)
        {
            return base.GetQueryable(context)
                .Include(x => x.User)
                .Include(x => x.Items)
                .Include(x => x.ReceivedMails)
                    .ThenInclude(x => x.Receiver)
                .Include(x => x.ReceivedMails)
                    .ThenInclude(x => x.Sender)
                .Include(x => x.ReceivedMails)
                    .ThenInclude(x => x.Item)
                .Include(x => x.SentMails)
                    .ThenInclude(x => x.Receiver)
                .Include(x => x.SentMails)
                    .ThenInclude(x => x.Sender)
                .Include(x => x.SentMails)
                    .ThenInclude(x => x.Item);
        }
    }
}
