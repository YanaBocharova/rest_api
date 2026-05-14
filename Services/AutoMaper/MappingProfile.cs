using AutoMapper;
using Domain.Entity;
using Services.Abstract.Dto;

namespace Services.AutoMaper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AccountDto, Account>();
            CreateMap<Account, AccountDto>();
        }
    }
}
