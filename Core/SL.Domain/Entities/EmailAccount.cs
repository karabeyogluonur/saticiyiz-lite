using System;
namespace SL.Domain.Entities
{
    public class EmailAccount : BaseEntity
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool EnableSsl { get; set; } = true;
        public bool UseDefaultCredentials { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}

