using SL.Application.Models.ViewModels.Account;

namespace SL.Application.Interfaces.Services.Membership
{
    public interface IRegistrationWorkflowService
    {
        Task ExecuteRegistrationAsync(RegisterViewModel registerViewModel);
    }
}
