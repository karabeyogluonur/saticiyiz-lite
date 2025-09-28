using AutoMapper;
using SL.Application.Models.ViewModels.EmailAccount;
using SL.Domain.Entities;

namespace SL.Application.Mappers
{
    public class EmailAccountMapperProfile : Profile
    {
        public EmailAccountMapperProfile()
        {
            CreateMap<EmailAccountAddViewModel, EmailAccount>().ReverseMap();
            CreateMap<EmailAccountListViewModel, EmailAccount>().ReverseMap();
            CreateMap<EmailAccountEditViewModel, EmailAccount>().ReverseMap();
        }
    }
}

