using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface IRepository<TValue>
        where TValue : BaseEntity
    {
        Task<IEnumerable<TValue>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<TValue>> GetAllAsync(Expression<Func<Account, bool>> predicate, CancellationToken cancellationToken);
        Task<TValue> GetAsync(int id, CancellationToken cancellationToken);
        Task<TValue> GetAsync(string email, string password, CancellationToken cancellationToken);
        Task<TValue> GetAsync(string email, CancellationToken cancellationToken);
        Task CreateAsync(TValue entity, CancellationToken cancellationToken);
        Task RemoveAsync(int id, CancellationToken cancellationToken);
        Task RemoveAsync(string email, CancellationToken cancellationToken);
        Task UpdateAsync(TValue entity, CancellationToken cancellationToken);
    }
}
