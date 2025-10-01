using System;
using SL.Application.Models.Request;
using SL.Application.Models.Response;
using SL.Application.Models.ViewModels.EmailTemplate;
using SL.Domain;
using SL.Domain.Entities.Messages;

namespace SL.Application.Interfaces.Services.Messages;

public interface IEmailTemplateService
{
    Task<Result<EmailTemplateEditViewModel>> GetTemplateForEditAsync(Guid emailTemplateId);

    Task<Result> UpdateTemplateAsync(EmailTemplateEditViewModel emailTemplateEditViewModel);

    Task<DataTablesResponse<EmailTemplateListViewModel>> GetEmailTemplatesForDataTablesAsync(DataTablesRequest dataTablesRequest);

    Task<Result> ToggleIsActiveAsync(Guid id);

    Task<EmailTemplate?> GetTemplateBySystemNameAsync(string systemName);

}
