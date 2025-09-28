using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SL.Application.Models.ViewModels.EmailTemplate;

public class EmailTemplateEditViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Şablon Adı")]
    public string Name { get; set; }

    [Display(Name = "Sistem Adı (Değiştirilemez)")]
    public string SystemName { get; set; }

    [Display(Name = "E-posta Konusu")]
    public string Subject { get; set; }

    [Display(Name = "E-posta İçeriği (HTML)")]
    public string Body { get; set; }

    [Display(Name = "Gizli Kopya (BCC) Adresleri")]
    public string? BccEmailAddresses { get; set; }

    [Display(Name = "Aktif mi?")]
    public bool IsActive { get; set; }

    [Display(Name = "Gönderim Gecikmesi (Saat)")]
    public int? SendDelayInHours { get; set; }

    [Display(Name = "Gönderici E-posta Hesabı")]
    public Guid EmailAccountId { get; set; }

    public SelectList? AvailableEmailAccounts { get; set; }
}
