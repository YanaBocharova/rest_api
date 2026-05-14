using Services.Abstract.Dto;

namespace Services.Abstract.Interfaces
{
    public interface IAccountsService
    {
        Task<IEnumerable<AccountDto>> GetAllAccounts(CancellationToken cancellationToken);
        Task<AccountDto> GetAccountById(int id, CancellationToken cancellationToken);
        Task<AccountDto> UpdateAccount(AccountDto category, CancellationToken cancellationToken);
        Task<AccountDto> CreateAccount(AccountDto dto, CancellationToken cancellationToken);
        Task<AccountDto> RemoveAccountById(int id, CancellationToken cancellationToken);
        Task<AccountDto> GetAccountByEmail(string email, CancellationToken cancellationToken);
        Task<AccountDto> GetAccountByEmail(string email, string password, CancellationToken cancellationToken);
        Task<AccountDto> RemoveAccountByEmail(string email, CancellationToken cancellationToken);
    }
}
