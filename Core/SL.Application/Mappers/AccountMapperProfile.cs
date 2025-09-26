using System;
using AutoMapper;
using SL.Application.Models.ViewModels.Account;
using SL.Domain.Entities;

namespace SL.Application.Mappers
{
    public class AccountMapperProfile : Profile
    {
        public AccountMapperProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>()
           .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
           .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
           .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
           .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}

