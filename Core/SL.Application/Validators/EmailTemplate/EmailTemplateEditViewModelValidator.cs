using System;
using System.Net.Mail;
using FluentValidation;
using SL.Application.Models.ViewModels.EmailTemplate;

namespace SL.Application.Validators.EmailTemplate;

public class EmailTemplateEditViewModelValidator : AbstractValidator<EmailTemplateEditViewModel>
{
    public EmailTemplateEditViewModelValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Güncellenecek kayıt kimliği (Id) boş olamaz.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Şablon Adı zorunludur.")
            .MaximumLength(250).WithMessage("Şablon Adı en fazla 250 karakter olabilir.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("E-posta Konusu zorunludur.");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("E-posta İçeriği zorunludur.");

        RuleFor(x => x.EmailAccountId)
            .NotEmpty().WithMessage("Gönderici E-posta Hesabı seçimi zorunludur.")
            .NotEqual(Guid.Empty).WithMessage("Lütfen geçerli bir gönderici hesabı seçin.");

        RuleFor(x => x.BccEmailAddresses)
            .Must(BeAValidEmailList).When(x => !string.IsNullOrWhiteSpace(x.BccEmailAddresses))
            .WithMessage("Lütfen geçerli, virgülle ayrılmış e-posta adresleri girin.");
    }

    private bool BeAValidEmailList(string? emailList)
    {
        if (string.IsNullOrWhiteSpace(emailList))
        {
            return true;
        }

        var emails = emailList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (!emails.Any()) return false;

        foreach (var email in emails)
        {
            if (!MailAddress.TryCreate(email, out _))
            {
                return false;
            }
        }
        return true;
    }
}
