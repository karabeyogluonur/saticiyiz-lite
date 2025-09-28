
using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using SL.Application.Interfaces.Repositories;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Application.Interfaces.Services;
using SL.Application.Models.DTOs.Tenant;
using SL.Application.Models.ViewModels.Account;
using SL.Domain;
using SL.Domain.Entities;
using SL.Domain.Enums;
using SL.Persistence.Contexts;


namespace SL.Persistence.Services
{
    public class RegistrationWorkflowService : IRegistrationWorkflowService
    {
        private readonly IAuthService _authService;
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<RegistrationWorkflowService> _logger;
        public RegistrationWorkflowService(IAuthService authService, ITenantService tenantService, IUnitOfWork<TenantDbContext> unitOfWork, IUserService userService, IMapper mapper, ILogger<RegistrationWorkflowService> logger)
        {
            _authService = authService;
            _tenantService = tenantService;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task ExecuteRegistrationAsync(RegisterViewModel registerViewModel)
        {
            Tenant tenant = null;
            string newDatabaseName = $"SL_{Guid.NewGuid():N}";
            try
            {
                var tenantCreateModel = new TenantCreateModel { DatabaseName = newDatabaseName };
                tenant = await _tenantService.InsertTenantAsync(tenantCreateModel);

                await _userService.CreateUserAsync(registerViewModel, tenant.Id, AppRole.User);
                await _tenantService.CreateDatabaseAsync(tenant);
            }
            catch (Exception ex)
            {
                if (tenant != null)
                {
                    _logger.LogCritical(ex, "Kayıt iş akışı kritik altyapı hatası nedeniyle geri alınıyor. Tenant: {TenantId}", tenant.Id);
                    await _tenantService.DeleteTenantAsync(tenant.Id);
                    await _userService.DeleteUserByTenantIdAsync(tenant.Id);
                }

                throw new ApplicationException("Sistem hatası nedeniyle kayıt tamamlanamadı.", ex);
            }
        }
    }

}

