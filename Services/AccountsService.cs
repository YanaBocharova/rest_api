using AutoMapper;
using Domain.Entity;
using Domain.Interfaces;
using Services.Abstract.Dto;
using Services.Abstract.Interfaces;

namespace Services
{
    internal class AccountsService : IAccountsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AccountsService(IUnitOfWork uow, IMapper map) 
        {
            unitOfWork = uow;
            mapper = map;
        }

        public async Task<IEnumerable<AccountDto>> GetAllAccounts(CancellationToken token)
        {
            var accounts = await unitOfWork.AccountsRepository.GetAllAsync(token);
            return mapper.Map<IEnumerable<AccountDto>>(accounts);
        }

        public async Task<AccountDto> GetAccountById(int id, CancellationToken token)
        {
            var srch = await unitOfWork.AccountsRepository.GetAsync(id, token);
            return mapper.Map<AccountDto>(srch);
        }

        public async Task<AccountDto> GetAccountByEmail(string email, string password, CancellationToken token)
        {
            var srch = await unitOfWork.AccountsRepository.GetAsync(email, token);
            return mapper.Map<AccountDto>(srch);
        }

        public async Task<AccountDto> GetAccountByEmail(string email, CancellationToken token)
        {
            var srch = await unitOfWork.AccountsRepository.GetAsync(email, token);
            return mapper.Map<AccountDto>(srch);
        }

        public async Task<AccountDto> RemoveAccountByEmail(string email, CancellationToken token)
        {
            await unitOfWork.AccountsRepository.RemoveAsync(email, token);
            await unitOfWork.SaveChangesAsync(token);
            return await GetAccountByEmail(email, token);
        }

        public async Task<AccountDto> RemoveAccountById(int id, CancellationToken token)
        {
            await unitOfWork.AccountsRepository.RemoveAsync(id, token);
            await unitOfWork.SaveChangesAsync(token);
            return await GetAccountById(id, token);
        }

        public async Task<AccountDto> UpdateAccount(AccountDto acc, CancellationToken token)
        {
            var accountToUpdate = mapper.Map<Account>(acc);
            await unitOfWork.AccountsRepository.UpdateAsync(accountToUpdate, token);
            await unitOfWork.SaveChangesAsync(token);
            return await GetAccountByEmail(accountToUpdate.Email, token);
        }

        public async Task<AccountDto> CreateAccount(AccountDto acc, CancellationToken token)
        {
            var account = mapper.Map<Account>(acc);
            await unitOfWork.AccountsRepository.CreateAsync(account, token);
            await unitOfWork.SaveChangesAsync(token);
            return await GetAccountByEmail(account.Email, token);
        }
    }
}
