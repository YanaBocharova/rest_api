using Domain.Entity;
using Domain.Interfaces;
using Persistence.Repositories;

namespace Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly DatabaseContext db;

        IRepository<Account>? _accountsRepository;
        public IRepository<Account> AccountsRepository => _accountsRepository ?? (_accountsRepository = new AccountsRepository(db));

        public UnitOfWork(DatabaseContext context)
        {
            db = context;
        }

        public void SaveChanges() => db.SaveChanges();
    }
}
