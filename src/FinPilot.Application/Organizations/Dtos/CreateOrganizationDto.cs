namespace FinPilot.Application.Organizations.Dtos;

public class CreateOrganizationDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string Currency { get; set; } = "USD";
    public string? TimeZone { get; set; }
}
