using System;
using FluentValidation;
using SL.Application.Models.ViewModels.EmailAccount;

namespace SL.Application.Validators.EmailAccount;

public class EmailAccountEditViewModelValidator : AbstractValidator<EmailAccountEditViewModel>
{
    public EmailAccountEditViewModelValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Kayıt kimliği (Id) boş olamaz.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Görünen Ad zorunludur.")
            .MaximumLength(250).WithMessage("Görünen Ad en fazla 250 karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi zorunludur.")
            .EmailAddress().WithMessage("Lütfen geçerli bir e-posta adresi girin.")
            .MaximumLength(250).WithMessage("E-posta adresi en fazla 250 karakter olabilir.");

        RuleFor(x => x.Host)
            .NotEmpty().WithMessage("Sunucu (Host) adresi zorunludur.")
            .MaximumLength(250).WithMessage("Sunucu adresi en fazla 250 karakter olabilir.");

        RuleFor(x => x.Port)
            .NotEmpty().WithMessage("Port numarası zorunludur.")
            .InclusiveBetween(1, 65535).WithMessage("Port numarası 1 ile 65535 arasında olmalıdır.");

        When(x => !x.UseDefaultCredentials, () =>
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Kullanıcı Adı zorunludur.");
        });
    }
}