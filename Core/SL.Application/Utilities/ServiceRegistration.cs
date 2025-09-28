using System.Globalization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace SL.Application.Utilities
{
    public static class ServiceRegistration
    {
        public static void AddApplicationService(this IServiceCollection services)
        {
            //services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.AddFluentValidation(x =>
            {
                x.DisableDataAnnotationsValidation = true;
                x.RegisterValidatorsFromAssemblyContaining<RegisterViewModelValidator>();
            });
            ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("tr-TR");
            services.AddFluentValidationAutoValidation();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}

