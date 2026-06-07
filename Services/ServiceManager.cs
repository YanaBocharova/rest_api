using AutoMapper;
using Domain.Interfaces;
using Services.Abstract.Interfaces;
using Services.AutoMaper;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        public IAccountsService AccountsService => _accountsService.Value;

        private readonly Lazy<IAccountsService> _accountsService;

        public ServiceManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _accountsService = new Lazy<IAccountsService>(() => new AccountsService(unitOfWork, mapper));
        }
    }
}
