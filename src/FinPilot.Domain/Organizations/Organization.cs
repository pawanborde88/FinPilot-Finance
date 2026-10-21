namespace FinPilot.Domain.Organizations;

public sealed class Organization
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; private set; }

    // Required by Entity Framework Core
    private Organization()
    {
    }

    public Organization(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Organization name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException(
                "Organization slug is required.", nameof(slug));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
