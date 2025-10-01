using SL.Application.Models.Request;
using SL.Application.Models.Response;
using SL.Application.Models.ViewModels.QueuedEmail;
using SL.Domain;
using SL.Domain.Entities.Messages;
using SL.Domain.Enums.Messages;

namespace SL.Application.Interfaces.Services.Messages;

public interface IQueuedEmailService
{
    Task<DataTablesResponse<QueuedEmailListViewModel>> GetQueuedEmailsForDataTablesAsync(DataTablesRequest dataTableRequest);
    Task<Result> DeleteQueuedEmailAsync(Guid queuedEmailId);
    
    Task<Result<Guid>> QueueEmailAsync(
        string toEmail, 
        string subject, 
        string body, 
        Guid emailAccountId, 
        EmailPriority priority = EmailPriority.Normal, 
        string? bcc = null, 
        string? cc = null);
    
    Task<Result<Guid>> QueueEmailFromTemplateAsync(
        string toEmail, 
        Guid templateId, 
        EmailPriority priority = EmailPriority.Normal, 
        string? bcc = null, 
        string? cc = null);
    
    Task<IEnumerable<QueuedEmail>> GetPendingEmailsAsync(int batchSize = 10);
    
    Task<Result> UpdateEmailStatusAsync(Guid queuedEmailId, int status, string? errorMessage = null);
}