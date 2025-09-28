using System;
using AutoMapper;
using SL.Application.Models.DTOs.Tenant;
using SL.Application.Models.ViewModels.Account;
using SL.Domain.Entities;
namespace SL.Application.Mappers
{
    public class TenantMapperProfile : Profile
    {
        public TenantMapperProfile()
        {
            CreateMap<TenantCreateModel, Tenant>();
        }
    }
}
