using System;
using System.ComponentModel.DataAnnotations;
namespace SL.Domain.Enums.Messages
{
    public enum QueuedEmailStatus
    {
        [Display(Name = "Beklemede")]
        Pending = 0,

        [Display(Name = "Gönderildi")]
        Sent = 1,

        [Display(Name = "Başarısız")]
        Failed = 2,

        [Display(Name = "İptal Edildi")]
        Aborted = 3
    }
}
