using Microsoft.EntityFrameworkCore;
using Rhisis.Database.Entities;
using System.Linq;

namespace Rhisis.Database.Repositories.Implementation
{
    /// <summary>
    /// User repository.
    /// </summary>
    internal sealed class UserRepository : RepositoryBase<DbUser>, IUserRepository
    {
        /// <summary>
        /// Creates and initialize an <see cref="UserRepository"/>.
        /// </summary>
        /// <param name="context"></param>
        public UserRepository(DbContext context)
            : base(context)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<DbUser> GetQueryable(DbContext context)
        {
            return base.GetQueryable(context)
                .Include(x => x.Characters)
                .ThenInclude(x => x.Items)
                .Include(x => x.Characters)
                .ThenInclude(x => x.Mails);
        }
    }
}
