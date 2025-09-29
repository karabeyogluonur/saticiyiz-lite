using AutoMapper;
using SL.Application.Models.DTOs.Tenant;
using SL.Application.Models.ViewModels.Account;
using SL.Domain.Entities.Membership;
namespace SL.Application.Mappers
{
    public class MembershipProfile : Profile
    {
        public MembershipProfile()
        {

            #region Tenant

            CreateMap<TenantCreateModel, Tenant>();

            #endregion

            #region Application User

            CreateMap<RegisterViewModel, ApplicationUser>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            #endregion

        }
    }
}
