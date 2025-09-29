
using AutoMapper;
using SL.Application.Models.ViewModels.EmailAccount;
using SL.Application.Models.ViewModels.EmailTemplate;
using SL.Domain.Entities.Messages;

namespace SL.Application.Mappers
{
    public class MessagesProfile : Profile
    {
        public MessagesProfile()
        {

            #region Email Account

            CreateMap<EmailAccountAddViewModel, EmailAccount>().ReverseMap();
            CreateMap<EmailAccountListViewModel, EmailAccount>().ReverseMap();
            CreateMap<EmailAccountEditViewModel, EmailAccount>().ReverseMap();

            #endregion

            #region Email Template

            CreateMap<EmailTemplateEditViewModel, EmailTemplate>()
                .ForMember(dest => dest.SystemName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<EmailTemplate, EmailTemplateEditViewModel>();

            CreateMap<EmailTemplate, EmailTemplateListViewModel>()
                .ForMember(dest => dest.EmailAccountName, opt => opt.MapFrom(src => src.EmailAccount.DisplayName));

            #endregion

        }
    }
}
