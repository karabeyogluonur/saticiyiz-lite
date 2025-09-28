using System;
using SL.Application.Models.Request;
using SL.Application.Models.Response;
using SL.Application.Models.ViewModels.EmailTemplate;
using SL.Domain;

namespace SL.Application.Interfaces.Services;

public interface IEmailTemplateService
{
    Task<Result<EmailTemplateEditViewModel>> GetTemplateForEditAsync(Guid emailTemplateId);

    Task<Result> UpdateTemplateAsync(EmailTemplateEditViewModel emailTemplateEditViewModel);

    Task<DataTablesResponse<EmailTemplateListViewModel>> GetTemplatesForDataTablesAsync(DataTablesRequest request);

    Task<Result> ToggleIsActiveAsync(Guid id);

}
