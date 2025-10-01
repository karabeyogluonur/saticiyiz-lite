using Microsoft.Extensions.Logging;
using SL.Application.Interfaces.Services.Messages;
using SL.Domain.Defaults.Messages;
using SL.Domain.Entities.Membership;
using SL.Domain.Enums.Messages;

namespace SL.Infrastructre.Services.Messages;

public class EmailWorkflowService : IEmailWorkflowService
{
    private readonly IMessageTemplateService _messageTemplateService;
    private readonly IQueuedEmailService _queuedEmailService;
    private readonly ILogger<EmailWorkflowService> _logger;

    public EmailWorkflowService(
        IMessageTemplateService messageTemplateService,
        IQueuedEmailService queuedEmailService,
        ILogger<EmailWorkflowService> logger)
    {
        _messageTemplateService = messageTemplateService;
        _queuedEmailService = queuedEmailService;
        _logger = logger;
    }

    public async Task<bool> SendCustomerWelcomeEmailAsync(ApplicationUser user)
    {
        try
        {
            _logger.LogInformation("Sending customer welcome email to {Email}", user.Email);
            
            return await SendTemplateEmailAsync(
                MessageTemplateSystemName.CustomerWelcome,
                user.Email,
                user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending customer welcome email to {Email}", user.Email);
            return false;
        }
    }

    public async Task<bool> SendCustomerPasswordRecoveryEmailAsync(ApplicationUser user, string resetToken)
    {
        try
        {
            _logger.LogInformation("Sending password recovery email to {Email}", user.Email);
            
            return await SendTemplateEmailAsync(
                MessageTemplateSystemName.CustomerPasswordRecovery,
                user.Email,
                user,
                resetToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password recovery email to {Email}", user.Email);
            return false;
        }
    }

    public async Task<bool> SendCustomerEmailValidationEmailAsync(ApplicationUser user, string validationToken)
    {
        try
        {
            _logger.LogInformation("Sending email validation email to {Email}", user.Email);
            
            return await SendTemplateEmailAsync(
                MessageTemplateSystemName.CustomerActivation,
                user.Email,
                user,
                validationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email validation email to {Email}", user.Email);
            return false;
        }
    }

    public async Task<bool> SendSystemAnnouncementEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            _logger.LogInformation("Sending system announcement email to {Email}", toEmail);
            
            var result = await _queuedEmailService.QueueEmailAsync(
                toEmail: toEmail,
                subject: subject,
                body: body,
                emailAccountId: Guid.Parse("00000000-0000-0000-0000-000000000001"), // Default email account
                priority: EmailPriority.Normal
            );

            if (result.IsSuccess)
            {
                _logger.LogInformation("System announcement email queued successfully for {Email}. QueuedEmail ID: {QueuedEmailId}", 
                    toEmail, result.Value);
                return true;
            }
            else
            {
                _logger.LogError("Failed to queue system announcement email for {Email}. Error: {Error}", 
                    toEmail, result.ErrorMessage);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending system announcement email to {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendTemplateEmailAsync(string systemName, string toEmail, params object[] entities)
    {
        try
        {
            _logger.LogInformation("Sending template email '{SystemName}' to {Email}", systemName, toEmail);
            
            var template = await _messageTemplateService.GetMessageTemplateAsync(systemName);
            if (template == null)
            {
                _logger.LogError("Message template not found: {SystemName}", systemName);
                return false;
            }

            var processedSubject = await _messageTemplateService.ReplaceTokensForSystemAsync(systemName, template.Subject, entities);
            var processedBody = await _messageTemplateService.ReplaceTokensForSystemAsync(systemName, template.Body, entities);

            _logger.LogInformation("Processed email subject: {Subject}", processedSubject);
            
            var result = await _queuedEmailService.QueueEmailAsync(
                toEmail: toEmail,
                subject: processedSubject,
                body: processedBody,
                emailAccountId: template.EmailAccountId,
                priority: EmailPriority.Normal
            );

            if (result.IsSuccess)
            {
                _logger.LogInformation("Template email '{SystemName}' queued successfully for {Email}. QueuedEmail ID: {QueuedEmailId}", 
                    systemName, toEmail, result.Value);
                return true;
            }
            else
            {
                _logger.LogError("Failed to queue template email '{SystemName}' for {Email}. Error: {Error}", 
                    systemName, toEmail, result.ErrorMessage);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending template email '{SystemName}' to {Email}", systemName, toEmail);
            return false;
        }
    }
}
