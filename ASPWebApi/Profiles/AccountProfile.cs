using ASPWebApi.Models;
using AutoMapper;
using Domain.Entity;
using Services.Abstract.Dto;

namespace ASPWebApi.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountDto, AccountModel>().ReverseMap();
        }
    }
}
