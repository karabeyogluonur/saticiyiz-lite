using SL.Domain.Enums.Messages;
namespace SL.Domain.Entities.Messages
{
    public class QueuedEmail : BaseEntity
    {
        public QueuedEmailStatus Status { get; set; } = QueuedEmailStatus.Pending;

        public EmailPriority Priority { get; set; } = EmailPriority.Normal;

        public string From { get; set; }

        public string To { get; set; }

        public string? Cc { get; set; }

        public string? Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public Guid EmailAccountId { get; set; }
        public virtual EmailAccount EmailAccount { get; set; }

        public int SentTries { get; set; } = 0;

        public DateTime? SentAt { get; set; }

        public DateTime? NextTryAt { get; set; }

        public string? LastError { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
