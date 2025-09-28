using System;
using SL.Application.Models.DTOs.Tenant;
using SL.Application.Models.ViewModels.Account;
using SL.Domain;
using SL.Domain.Entities;
namespace SL.Application.Interfaces.Services
{
    public interface IRegistrationWorkflowService
    {
        Task ExecuteRegistrationAsync(RegisterViewModel registerViewModel);
    }
}
