using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SL.Application.Interfaces.Repositories;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Application.Interfaces.Services.Messages;
using SL.Application.Interfaces.Services.Security;
using SL.Application.Models.Request;
using SL.Application.Models.Response;
using SL.Application.Models.ViewModels.EmailAccount;
using SL.Domain;
using SL.Domain.Entities.Messages;
using SL.Domain.Enums.Common;
using SL.Persistence.Contexts;
using SL.Persistence.Extensions;
using System.Linq.Dynamic.Core;

namespace SL.Persistence.Services.Messages;

public class EmailAccountService : IEmailAccountService
{
    private readonly IUnitOfWork<MasterDbContext> _masterUnitOfWork;
    private readonly IRepository<EmailAccount> _emailAccountRepository;
    private readonly IMapper _mapper;
    private readonly IDataProtectionService _dataProtectionService;
    private readonly ILogger<EmailAccountService> _logger;
    public EmailAccountService(IUnitOfWork<MasterDbContext> masterUnitOfWork, IMapper mapper, IPasswordHasherService passwordHasherService, ILogger<EmailAccountService> logger, IDataProtectionService dataProtectionService)
    {
        _masterUnitOfWork = masterUnitOfWork;
        _emailAccountRepository = _masterUnitOfWork.GetRepository<EmailAccount>();
        _mapper = mapper;
        _dataProtectionService = dataProtectionService;
    }
    public async Task<Result> CreateEmailAccountAsync(EmailAccountAddViewModel emailAccountAddViewModel)
    {
        var emailExists = await _emailAccountRepository.ExistsAsync(emailAccount => emailAccount.Email.ToLower() == emailAccountAddViewModel.Email.ToLower());

        if (emailExists)
            return Result.Failure("Bu e-posta adresi zaten kullanılıyor.", ErrorCode.ValidationFailure);

        EmailAccount emailAccount = _mapper.Map<EmailAccount>(emailAccountAddViewModel);

        if (!string.IsNullOrWhiteSpace(emailAccountAddViewModel.Password))
            emailAccount.Password = _dataProtectionService.Encrypt(emailAccountAddViewModel.Password);

        await _emailAccountRepository.InsertAsync(emailAccount);
        await _masterUnitOfWork.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<Result> DeleteEmailAccountAsync(Guid emailAccountId)
    {
        try
        {
            var accountToDelete = await _emailAccountRepository.FindAsync(emailAccountId);
            if (accountToDelete == null)
                return Result.Failure("Silinecek e-posta hesabı bulunamadı.", ErrorCode.ResourceNotFound);
            accountToDelete.IsDeleted = true;
            accountToDelete.DeletedAt = DateTime.UtcNow;
            _emailAccountRepository.Update(accountToDelete);
            await _masterUnitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError("Email Account silme işleminde bir hata meydana geldi. Id : {0}", emailAccountId);
            return Result.Failure("Silme işlemi sırasında beklenmedik bir sunucu hatası oluştu.", ErrorCode.InternalServerError);
        }
    }
    public async Task<IEnumerable<EmailAccount>> GetAllAsync()
    {
        return await _emailAccountRepository.GetAllAsync();
    }

    public async Task<Result<EmailAccountEditViewModel>> GetEmailAccountForEditAsync(Guid id)
    {
        EmailAccount accountEntity = await _emailAccountRepository.FindAsync(id);
        if (accountEntity == null)
            return Result<EmailAccountEditViewModel>.Failure("Hesap bulunamadı.", ErrorCode.ResourceNotFound);
        EmailAccountEditViewModel emailAccountEditViewModel = _mapper.Map<EmailAccountEditViewModel>(accountEntity);
        return Result<EmailAccountEditViewModel>.Success(emailAccountEditViewModel);
    }
    public async Task<DataTablesResponse<EmailAccountListViewModel>> GetEmailAccountsForDataTablesAsync(DataTablesRequest dataTablesRequest)
    {
        IOrderedQueryable<EmailAccount> query = _emailAccountRepository.GetAll().OrderByDescending(emailAccount => emailAccount.CreatedAt);
        return await query.ToDataTablesResponseAsync<EmailAccount, EmailAccountListViewModel>(dataTablesRequest, _mapper);
    }
    public async Task<Result> UpdateEmailAccountAsync(EmailAccountEditViewModel emailAccountEditViewModel)
    {
        var emailExistsOnOtherAccount = await _emailAccountRepository.ExistsAsync(x => x.Email.ToLower() == emailAccountEditViewModel.Email.ToLower() && x.Id != emailAccountEditViewModel.Id);
        if (emailExistsOnOtherAccount)
            return Result.Failure("Bu e-posta adresi zaten başka bir hesap tarafından kullanılıyor.", ErrorCode.ValidationFailure);
        try
        {
            var existingAccount = await _emailAccountRepository.FindAsync(emailAccountEditViewModel.Id);
            if (existingAccount == null)
                return Result.Failure("Güncellenecek e-posta hesabı bulunamadı.", ErrorCode.None);
            _mapper.Map(emailAccountEditViewModel, existingAccount);
            if (!string.IsNullOrWhiteSpace(emailAccountEditViewModel.Password))
                existingAccount.Password = _dataProtectionService.Encrypt(emailAccountEditViewModel.Password);
            await _masterUnitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email hesabı güncellenemedi. Email Id : {0}", emailAccountEditViewModel.Id);
            return Result.Failure("Güncelleme sırasında beklenmedik bir sunucu hatası oluştu.", ErrorCode.InternalServerError);
        }
    }
}
