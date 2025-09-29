using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SL.Application.Interfaces.Repositories;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Application.Models.Request;
using SL.Application.Models.Response;
using SL.Application.Models.ViewModels.EmailTemplate;
using SL.Domain;
using SL.Persistence.Contexts;
using System.Linq.Dynamic.Core;
using SL.Application.Interfaces.Services.Messages;
using SL.Domain.Entities.Messages;
using SL.Domain.Enums.Common;

namespace SL.Persistence.Services.Messages;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IUnitOfWork<MasterDbContext> _masterUnitOfWork;
    private readonly IRepository<EmailTemplate> _emailTemplateRepository;
    private readonly IEmailAccountService _emailAccountService;
    private readonly ILogger<EmailTemplateService> _logger;
    private readonly IMapper _mapper;

    public EmailTemplateService(IUnitOfWork<MasterDbContext> masterUnitOfWork, IEmailAccountService emailAccountService, IMapper mapper, ILogger<EmailTemplateService> logger)
    {
        _masterUnitOfWork = masterUnitOfWork;
        _emailTemplateRepository = _masterUnitOfWork.GetRepository<EmailTemplate>();
        _emailAccountService = emailAccountService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<EmailTemplateEditViewModel>> GetTemplateForEditAsync(Guid emailTemplateId)
    {
        EmailTemplate emailTemplate = await _emailTemplateRepository.FindAsync(emailTemplateId);

        if (emailTemplate == null)
            return Result<EmailTemplateEditViewModel>.Failure("Düzenlenecek e-posta şablonu bulunamadı.", ErrorCode.ResourceNotFound);

        EmailTemplateEditViewModel emailTemplateEditViewModel = _mapper.Map<EmailTemplateEditViewModel>(emailTemplate);

        IEnumerable<EmailAccount> accounts = await _emailAccountService.GetAllAsync();
        emailTemplateEditViewModel.AvailableEmailAccounts = new SelectList(accounts, "Id", "DisplayName", emailTemplateEditViewModel.EmailAccountId);

        return Result<EmailTemplateEditViewModel>.Success(emailTemplateEditViewModel);
    }

    public async Task<Result> UpdateTemplateAsync(EmailTemplateEditViewModel emailTemplateEditViewModel)
    {
        try
        {
            EmailTemplate existingTemplate = await _emailTemplateRepository.FindAsync(emailTemplateEditViewModel.Id);

            if (existingTemplate == null)
                return Result.Failure("Güncellenecek e-posta şablonu bulunamadı.", ErrorCode.ResourceNotFound);

            if (existingTemplate.SystemName != emailTemplateEditViewModel.SystemName)
            {
                return Result.Failure("Sistem adı değiştirilemez.", ErrorCode.ValidationFailure);
            }

            _mapper.Map(emailTemplateEditViewModel, existingTemplate);
            existingTemplate.UpdatedAt = DateTime.UtcNow;

            await _masterUnitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EmailTemplate güncellenirken hata oluştu. Id:{0}", emailTemplateEditViewModel.Id);
            return Result.Failure("Güncelleme sırasında beklenmedik bir sunucu hatası oluştu.", ErrorCode.InternalServerError);
        }
    }
    public async Task<DataTablesResponse<EmailTemplateListViewModel>> GetTemplatesForDataTablesAsync(DataTablesRequest request)
    {
        var query = _emailTemplateRepository.GetAll(include: emailTemplate => emailTemplate.Include(emailTemplate => emailTemplate.EmailAccount));

        // Global Arama
        if (!string.IsNullOrWhiteSpace(request.SearchValue))
        {
            var searchValue = request.SearchValue.ToLower();
            query = query.Where(et => et.Name.ToLower().Contains(searchValue) ||
                                              et.SystemName.ToLower().Contains(searchValue) ||
                                              et.Subject.ToLower().Contains(searchValue) ||
                                              et.EmailAccount.DisplayName.ToLower().Contains(searchValue));
        }

        int recordsTotal = await query.CountAsync();

        // Sıralama
        if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortDirection))
            query = query.OrderBy(request.SortColumn + " " + request.SortDirection);
        else
            query = query.OrderByDescending(et => et.CreatedAt);


        var pagedData = await query.Skip(request.Start).Take(request.Length).ToListAsync();

        var viewModelData = _mapper.Map<List<EmailTemplateListViewModel>>(pagedData);

        return new DataTablesResponse<EmailTemplateListViewModel>
        {
            Draw = request.Draw,
            RecordsTotal = recordsTotal,
            RecordsFiltered = recordsTotal,
            Data = viewModelData
        };
    }

    public async Task<Result> ToggleIsActiveAsync(Guid id)
    {
        try
        {
            var template = await _emailTemplateRepository.GetAll(ignoreQueryFilters: true).FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
                return Result.Failure("Durumu değiştirilecek şablon bulunamadı.", ErrorCode.ResourceNotFound);

            template.IsActive = !template.IsActive;
            template.UpdatedAt = DateTime.UtcNow;

            _emailTemplateRepository.Update(template);
            await _masterUnitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Durum değiştirilirken beklenmedik bir hata oluştu. Id : {0}", id);
            return Result.Failure("Durum değiştirilirken beklenmedik bir hata oluştu.", ErrorCode.InternalServerError);
        }
    }
}
