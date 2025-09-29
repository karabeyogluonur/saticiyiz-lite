using SL.Application.Models.Request;
using SL.Application.Models.Response;
using SL.Application.Models.ViewModels.QueuedEmail;
using SL.Domain;

namespace SL.Application.Interfaces.Services.Messages;

public interface IQueuedEmailService
{
    Task<DataTablesResponse<QueuedEmailListViewModel>> GetQueuedEmailsForDataTablesAsync(DataTablesRequest dataTableRequest);
    Task<Result> DeleteQueuedEmailAsync(Guid queuedEmailId);
}