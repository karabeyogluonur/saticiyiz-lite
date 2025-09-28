using SL.Application.Models.Request;
using SL.Application.Models.Response;
using SL.Application.Models.ViewModels.EmailAccount;
using SL.Domain;
using SL.Domain.Entities;

namespace SL.Application.Interfaces.Services;

public interface IEmailAccountService
{
    Task<DataTablesResponse<EmailAccountListViewModel>> GetEmailAccountsForDataTablesAsync(DataTablesRequest dataTablesRequest);

    Task<Result> CreateEmailAccountAsync(EmailAccountAddViewModel emailAccountAddViewModel);

    Task<Result> UpdateEmailAccountAsync(EmailAccountEditViewModel emailAccountEditViewModel);

    Task<Result> DeleteEmailAccountAsync(Guid emailAccountId);

    Task<Result<EmailAccountEditViewModel>> GetEmailAccountForEditAsync(Guid id);
}
