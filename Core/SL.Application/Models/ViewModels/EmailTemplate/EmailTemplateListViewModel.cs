using System;

namespace SL.Application.Models.ViewModels.EmailTemplate;

public class EmailTemplateListViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SystemName { get; set; }
    public string Subject { get; set; }
    public bool IsActive { get; set; }
    public string EmailAccountName { get; set; }
}
