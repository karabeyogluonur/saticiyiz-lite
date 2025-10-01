using SL.Domain.Entities.Membership;

namespace SL.Application.Interfaces.Services.Messages;

public interface IEmailWorkflowService
{
    Task<bool> SendCustomerWelcomeEmailAsync(ApplicationUser user);

    Task<bool> SendCustomerPasswordRecoveryEmailAsync(ApplicationUser user, string resetToken);

    Task<bool> SendCustomerEmailValidationEmailAsync(ApplicationUser user, string validationToken);

    Task<bool> SendSystemAnnouncementEmailAsync(string toEmail, string subject, string body);

    Task<bool> SendTemplateEmailAsync(string systemName, string toEmail, params object[] entities);
}
