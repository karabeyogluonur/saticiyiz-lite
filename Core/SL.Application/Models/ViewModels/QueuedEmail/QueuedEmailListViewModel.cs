using Microsoft.AspNetCore.Mvc.Rendering;
using SL.Domain.Enums.Messages;


namespace SL.Application.Models.ViewModels.QueuedEmail
{
    public class QueuedEmailListViewModel
    {
        public Guid Id { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string EmailAccountName { get; set; }
        public DateTime CreatedAt { get; set; }
        public QueuedEmailStatus Status { get; set; }
        public int SentTries { get; set; }

        public string StatusDisplayName { get; set; }
        public IEnumerable<SelectListItem> AvailableStatuses { get; set; }
        public IEnumerable<SelectListItem> AvailableEmailAccounts { get; set; }
    }
}