using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SL.Application.Interfaces.Repositories;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Application.Interfaces.Services.Messages;
using SL.Application.Models.Request;
using SL.Application.Models.Response;
using SL.Application.Models.ViewModels.QueuedEmail;
using SL.Domain;
using SL.Domain.Entities.Messages;
using SL.Domain.Enums.Common;
using SL.Domain.Enums.Messages;
using SL.Persistence.Contexts;
using SL.Persistence.Extensions;
using System.Linq.Dynamic.Core;


namespace SL.Persistence.Services.Messages
{
    public class QueuedEmailService : IQueuedEmailService
    {
        private readonly IUnitOfWork<MasterDbContext> _masterUnitOfWork;
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly ILogger<QueuedEmailService> _logger;
        private readonly IMapper _mapper;

        public QueuedEmailService(IUnitOfWork<MasterDbContext> masterUnitOfWork, ILogger<QueuedEmailService> logger, IMapper mapper)
        {
            _masterUnitOfWork = masterUnitOfWork;
            _queuedEmailRepository = _masterUnitOfWork.GetRepository<QueuedEmail>();
            _emailAccountRepository = _masterUnitOfWork.GetRepository<EmailAccount>();
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<DataTablesResponse<QueuedEmailListViewModel>> GetQueuedEmailsForDataTablesAsync(DataTablesRequest dataTableRequest)
        {
            IOrderedQueryable<QueuedEmail> query = _queuedEmailRepository.GetAll(include: queuedEmail => queuedEmail.Include(e => e.EmailAccount)).OrderBy(queuedEmail => queuedEmail.Status).ThenByDescending(queuedEmail => queuedEmail.CreatedAt);
            return await query.ToDataTablesResponseAsync<QueuedEmail, QueuedEmailListViewModel>(dataTableRequest, _mapper);
        }

        public async Task<Result> DeleteQueuedEmailAsync(Guid queuedEmailId)
        {
            try
            {
                var emailToDelete = await _queuedEmailRepository.FindAsync(queuedEmailId);

                if (emailToDelete == null)
                {
                    return Result.Failure("Silinecek e-posta bulunamadı.", ErrorCode.ResourceNotFound);
                }

                _queuedEmailRepository.Delete(emailToDelete);
                await _masterUnitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QueuedEmail (ID: {QueuedEmailId}) silinirken bir hata oluştu.", queuedEmailId);
                return Result.Failure("E-posta silinirken beklenmedik bir sunucu hatası oluştu.", ErrorCode.InternalServerError);
            }
        }

        public async Task<Result<Guid>> QueueEmailAsync(
            string toEmail, 
            string subject, 
            string body, 
            Guid emailAccountId, 
            EmailPriority priority = EmailPriority.Normal, 
            string? bcc = null, 
            string? cc = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmail))
                    return Result<Guid>.Failure("Alıcı e-posta adresi boş olamaz.", ErrorCode.ValidationFailure);

                if (string.IsNullOrWhiteSpace(subject))
                    return Result<Guid>.Failure("E-posta konusu boş olamaz.", ErrorCode.ValidationFailure);

                if (string.IsNullOrWhiteSpace(body))
                    return Result<Guid>.Failure("E-posta içeriği boş olamaz.", ErrorCode.ValidationFailure);

                var emailAccount = await _emailAccountRepository.FindAsync(emailAccountId);
                if (emailAccount == null)
                    return Result<Guid>.Failure("Email hesabı bulunamadı.", ErrorCode.ResourceNotFound);

                var queuedEmail = new QueuedEmail
                {
                    Id = Guid.NewGuid(),
                    From = emailAccount.Email,
                    To = toEmail.Trim(),
                    Subject = subject.Trim(),
                    Body = body,
                    EmailAccountId = emailAccountId,
                    Priority = priority,
                    Bcc = bcc?.Trim(),
                    Cc = cc?.Trim(),
                    Status = QueuedEmailStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    SentAt = null,
                    LastError = null
                };

                _queuedEmailRepository.Insert(queuedEmail);
                await _masterUnitOfWork.SaveChangesAsync();

                _logger.LogInformation("Email queued successfully. ID: {QueuedEmailId}, To: {ToEmail}", 
                    queuedEmail.Id, toEmail);

                return Result<Guid>.Success(queuedEmail.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error queuing email to {ToEmail}", toEmail);
                return Result<Guid>.Failure("E-posta kuyruğa eklenirken bir hata oluştu.", ErrorCode.InternalServerError);
            }
        }

        public async Task<Result<Guid>> QueueEmailFromTemplateAsync(
            string toEmail, 
            Guid templateId, 
            EmailPriority priority = EmailPriority.Normal, 
            string? bcc = null, 
            string? cc = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmail))
                    return Result<Guid>.Failure("Alıcı e-posta adresi boş olamaz.", ErrorCode.ValidationFailure);

                var templateRepository = _masterUnitOfWork.GetRepository<EmailTemplate>();
                var template = await templateRepository.FindAsync(templateId);

                if (template == null)
                    return Result<Guid>.Failure("E-posta şablonu bulunamadı.", ErrorCode.ResourceNotFound);

                if (!template.IsActive)
                    return Result<Guid>.Failure("E-posta şablonu aktif değil.", ErrorCode.ValidationFailure);

                var emailAccount = await _emailAccountRepository.FindAsync(template.EmailAccountId);
                if (emailAccount == null)
                    return Result<Guid>.Failure("Email hesabı bulunamadı.", ErrorCode.ResourceNotFound);

                var queuedEmail = new QueuedEmail
                {
                    Id = Guid.NewGuid(),
                    From = emailAccount.Email,
                    To = toEmail.Trim(),
                    Subject = template.Subject,
                    Body = template.Body,
                    EmailAccountId = template.EmailAccountId,
                    Priority = priority,
                    Bcc = bcc?.Trim(),
                    Cc = cc?.Trim(),
                    Status = QueuedEmailStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    SentAt = null,
                    LastError = null
                };

                _queuedEmailRepository.Insert(queuedEmail);
                await _masterUnitOfWork.SaveChangesAsync();

                _logger.LogInformation("Email queued from template successfully. ID: {QueuedEmailId}, To: {ToEmail}, Template: {TemplateName}", 
                    queuedEmail.Id, toEmail, template.Name);

                return Result<Guid>.Success(queuedEmail.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error queuing email from template {TemplateId} to {ToEmail}", templateId, toEmail);
                return Result<Guid>.Failure("E-posta şablonundan kuyruğa eklenirken bir hata oluştu.", ErrorCode.InternalServerError);
            }
        }

        public async Task<IEnumerable<QueuedEmail>> GetPendingEmailsAsync(int batchSize = 10)
        {
            try
            {
                return await _queuedEmailRepository
                    .GetAll(include: q => q.Include(e => e.EmailAccount))
                    .Where(q => q.Status == QueuedEmailStatus.Pending)
                    .OrderBy(q => q.Priority)
                    .ThenBy(q => q.CreatedAt)
                    .Take(batchSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending emails");
                return Enumerable.Empty<QueuedEmail>();
            }
        }

        public async Task<Result> UpdateEmailStatusAsync(Guid queuedEmailId, int status, string? errorMessage = null)
        {
            try
            {
                var queuedEmail = await _queuedEmailRepository.FindAsync(queuedEmailId);
                if (queuedEmail == null)
                    return Result.Failure("Kuyruktaki e-posta bulunamadı.", ErrorCode.ResourceNotFound);

                queuedEmail.Status = (QueuedEmailStatus)status;
                queuedEmail.LastError = errorMessage;

                if (status == (int)QueuedEmailStatus.Sent)
                    queuedEmail.SentAt = DateTime.UtcNow;
                else if (status == (int)QueuedEmailStatus.Failed)
                    queuedEmail.SentAt = DateTime.UtcNow;

                await _masterUnitOfWork.SaveChangesAsync();

                _logger.LogInformation("Email status updated. ID: {QueuedEmailId}, Status: {Status}", 
                    queuedEmailId, status);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating email status for ID: {QueuedEmailId}", queuedEmailId);
                return Result.Failure("E-posta durumu güncellenirken bir hata oluştu.", ErrorCode.InternalServerError);
            }
        }
    }
}