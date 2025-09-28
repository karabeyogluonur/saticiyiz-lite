using FluentValidation;

namespace SL.Application.Models.ViewModels.EmailAccount.Validation;

public class EmailAccountAddViewModelValidator : AbstractValidator<EmailAccountAddViewModel>
{
    public EmailAccountAddViewModelValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Görünen Ad (Display Name) zorunludur.")
            .MaximumLength(250).WithMessage("Görünen Ad en fazla 250 karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
            .MaximumLength(250).WithMessage("E-posta adresi en fazla 250 karakter olabilir.");

        RuleFor(x => x.Host)
            .NotEmpty().WithMessage("Sunucu (Host) adresi zorunludur.")
            .MaximumLength(250).WithMessage("Sunucu adresi en fazla 250 karakter olabilir.");

        RuleFor(x => x.Port)
            .InclusiveBetween(1, 65535).WithMessage("Port numarası 1 ile 65535 arasında olmalıdır.");

        When(x => !x.UseDefaultCredentials, () =>
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Varsayılan kimlik bilgileri kullanılmıyorsa Kullanıcı Adı zorunludur.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Varsayılan kimlik bilgileri kullanılmıyorsa Şifre zorunludur.");
        });

    }
}