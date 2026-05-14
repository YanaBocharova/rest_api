using AutoMapper;
using Domain.Interfaces;
using Services.Abstract.Interfaces;
using Services.AutoMaper;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        public IAccountsService AccountsService => _accounntsService.Value;

        private readonly Lazy<IAccountsService> _accounntsService;

        public ServiceManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            mapper = mappingConfig.CreateMapper();

            _accounntsService = new Lazy<IAccountsService>(() => new AccountsService(unitOfWork, mapper));
        }
    }
}
