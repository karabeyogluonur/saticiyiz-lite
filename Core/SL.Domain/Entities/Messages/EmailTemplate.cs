using System;
using System.ComponentModel.DataAnnotations;

namespace SL.Domain.Entities.Messages
{
    public class EmailTemplate : BaseEntity
    {

        [Display(Name = "Şablon Adı")]
        public string Name { get; set; }

        [Display(Name = "Sistem Adı (Değiştirilemez)")]
        public string SystemName { get; set; }

        [Display(Name = "E-posta Konusu")]
        public string Subject { get; set; } = null!;

        [Display(Name = "E-posta İçeriği (HTML)")]
        public string Body { get; set; } = null!;

        [Display(Name = "Gizli Kopya (BCC) Adresleri")]
        public string? BccEmailAddresses { get; set; }

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Gönderici E-posta Hesabı")]
        public Guid EmailAccountId { get; set; }

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Son Güncellenme Tarihi")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual EmailAccount EmailAccount { get; set; } = null!;
    }
}
