using System;
using System.ComponentModel.DataAnnotations;
namespace SL.Application.Models.ViewModels.EmailAccount;

public class EmailAccountAddViewModel
{
    [Display(Name = "Görünen Ad")]
    public string DisplayName { get; set; }
    [Display(Name = "E-posta Adresi")]
    public string Email { get; set; }
    [Display(Name = "Sunucu (Host)")]
    public string Host { get; set; }
    [Display(Name = "Port")]
    public int Port { get; set; }
    [Display(Name = "Varsayılan Kimlik Bilgilerini Kullan")]
    public bool UseDefaultCredentials { get; set; }
    [Display(Name = "Kullanıcı Adı")]
    public string? Username { get; set; }
    [Display(Name = "Şifre")]
    public string? Password { get; set; }
    [Display(Name = "SSL'i Etkinleştir")]
    public bool EnableSsl { get; set; } = true;
}
