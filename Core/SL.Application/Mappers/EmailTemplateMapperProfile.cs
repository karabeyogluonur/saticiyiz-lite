using System;
using AutoMapper;
using SL.Application.Models.ViewModels.EmailTemplate;
using SL.Domain.Entities;

namespace SL.Application.Mappers;

public class EmailTemplateMapperProfile : Profile
{
    public EmailTemplateMapperProfile()
    {

        CreateMap<EmailTemplateEditViewModel, EmailTemplate>()
                .ForMember(dest => dest.SystemName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<EmailTemplate, EmailTemplateEditViewModel>();

        CreateMap<EmailTemplate, EmailTemplateListViewModel>()
            .ForMember(dest => dest.EmailAccountName, opt => opt.MapFrom(src => src.EmailAccount.DisplayName));
    }
}
