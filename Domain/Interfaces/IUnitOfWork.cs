using Domain.Entity;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Account> AccountsRepository { get; }
        void SaveChanges();
    }
}
