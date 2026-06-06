using AutoMapper;
using Domain.Entity;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Services.Abstract.Dto;
using Services.Abstract.Interfaces;

namespace Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly PasswordHasher<Account> hasher;

        public AccountsService(IUnitOfWork uow, IMapper map)
        {
            unitOfWork = uow;
            mapper = map;
            hasher = new PasswordHasher<Account>();
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
            var account = await unitOfWork.AccountsRepository.GetAsync(email, token);
            if (account == null)
                return null;

            var result = hasher.VerifyHashedPassword(
                account,
                account.Password,
                password
            );

            if (result == PasswordVerificationResult.Failed)
                return null;

            return mapper.Map<AccountDto>(account);
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

            // IMPORTANT: do not overwrite password here
            var existing = await unitOfWork.AccountsRepository.GetAsync(accountToUpdate.Email, token);
            accountToUpdate.Password = existing.Password;

            await unitOfWork.AccountsRepository.UpdateAsync(accountToUpdate, token);
            await unitOfWork.SaveChangesAsync(token);

            return await GetAccountByEmail(accountToUpdate.Email, token);
        }

        public async Task<AccountDto> CreateAccount(AccountDto acc, CancellationToken token)
        {
            var account = mapper.Map<Account>(acc);
            account.Password = hasher.HashPassword(account, account.Password);
            await unitOfWork.AccountsRepository.CreateAsync(account, token);
            await unitOfWork.SaveChangesAsync(token);

            return await GetAccountByEmail(account.Email, token);
        }
    }
}