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
using SL.Persistence.Contexts;
using SL.Persistence.Extensions;
using System.Linq.Dynamic.Core;


namespace SL.Persistence.Services.Messages
{
    public class QueuedEmailService : IQueuedEmailService
    {
        private readonly IUnitOfWork<MasterDbContext> _masterUnitOfWork;
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;
        private readonly ILogger<QueuedEmailService> _logger;
        private readonly IMapper _mapper;

        public QueuedEmailService(IUnitOfWork<MasterDbContext> masterUnitOfWork, ILogger<QueuedEmailService> logger, IMapper mapper)
        {
            _masterUnitOfWork = masterUnitOfWork;
            _queuedEmailRepository = _masterUnitOfWork.GetRepository<QueuedEmail>();
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
    }
}