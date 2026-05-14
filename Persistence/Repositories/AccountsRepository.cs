using Domain.Entity;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class AccountsRepository : IRepository<Account>
    {
        private readonly DatabaseContext db;
        protected DbSet<Account> Table => db.Set<Account>();

        public AccountsRepository(DatabaseContext context)
        {
            db = context;
        }

        public async Task CreateAsync(Account entity, CancellationToken cancellationToken)
        {
            await Table.AddAsync(entity, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await Table
                .AsNoTracking()
                .OrderBy(acc => acc.Email)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Account>> GetAllAsync(
            Expression<Func<Account, bool>> predicate,
            CancellationToken cancellationToken)
        {
            return await Table
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Account?> GetAsync(int id, CancellationToken cancellationToken)
        {
            return await Table
                .AsNoTracking()
                .FirstOrDefaultAsync(acc => acc.Id == id, cancellationToken);
        }

        public async Task<Account?> GetAsync(string email, string password, CancellationToken cancellationToken)
        {
            return await Table
                .AsNoTracking()
                .FirstOrDefaultAsync(acc => acc.Email == email && acc.Password == password, cancellationToken);
        }

        public async Task RemoveAsync(int id, CancellationToken cancellationToken)
        {
            await Table
                .Where(acc => acc.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task RemoveAsync(string email, CancellationToken cancellationToken)
        {
            await Table
                .Where(acc => acc.Email == email)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task UpdateAsync(Account entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Table
                .Where(acc => acc.Id == entity.Id)
                .ExecuteUpdateAsync(acc => acc
                    .SetProperty(a => a.Email, entity.Email)
                    .SetProperty(a => a.Password, entity.Password),
                    cancellationToken);
        }

        public async Task<Account> GetAsync(string email, CancellationToken cancellationToken)
        {
            return await Table
                .AsNoTracking()
                .FirstOrDefaultAsync(acc => acc.Email == email, cancellationToken);
        }
    }
}
