using FluentValidation;
using SL.Application.Models.ViewModels.Account;

public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
{
    public RegisterViewModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad alanı boş olamaz.")
            .Length(2, 50).WithMessage("Ad 2 ile 50 karakter arasında olmalıdır.")
            .Matches(@"^[a-zA-ZÇçĞğİıÖöŞşÜü\s]+$")
                .WithMessage("Ad yalnızca harflerden oluşmalıdır.");


        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad alanı boş olamaz.")
            .Length(2, 50).WithMessage("Soyad 2 ile 50 karakter arasında olmalıdır.")
            .Matches(@"^[a-zA-ZÇçĞğİıÖöŞşÜü\s]+$")
                .WithMessage("Soyad yalnızca harflerden oluşmalıdır.");


        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
            .MaximumLength(100).WithMessage("Email 100 karakteri aşamaz.");


        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
            .Matches(@"^\+?\d{10,15}$")
                .WithMessage("Telefon numarası uluslararası formatta olmalıdır.");


        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
            .MaximumLength(30).WithMessage("Şifre en fazla 30 karakter olmalıdır.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Şifre tekrar alanı boş olamaz.")
            .Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor.");
    }
}
