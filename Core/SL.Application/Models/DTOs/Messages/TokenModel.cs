namespace SL.Application.Models.DTOs.Messages;

public class TokenModel
{
    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsSystemToken { get; set; }

    public string Category { get; set; } = string.Empty;
}
