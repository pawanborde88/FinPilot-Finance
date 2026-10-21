using FinPilot.Domain.Common;

namespace FinPilot.Domain.Organizations;

public class Organization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string Currency { get; set; } = "USD";
    public string? TimeZone { get; set; }
    public bool IsActive { get; set; } = true;
}
