using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rhisis.Database.Entities;

namespace Rhisis.Database.Repositories.Implementation
{
    /// <summary>
    /// Mail repository.
    /// </summary>
    internal sealed class MailRepository : RepositoryBase<DbMail>, IMailRepository
    {
        /// <summary>
        /// Creates an initialize an <see cref="MailRepository"/>.
        /// </summary>
        /// <param name="context"></param>
        public MailRepository(DbContext context)
            : base(context)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<DbMail> GetQueryable(DbContext context)
        {
            return base.GetQueryable(context)
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .Include(x => x.Item);
        }
    }
}
