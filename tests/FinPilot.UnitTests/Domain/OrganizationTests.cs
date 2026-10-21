using FinPilot.Domain.Organizations;
using Xunit;

namespace FinPilot.UnitTests.Domain;

public class OrganizationTests
{
    [Fact]
    public void Constructor_WithValidArguments_InitializesOrganizationCorrectly()
    {
        // Arrange
        var name = "  Acme Corporation  ";
        var slug = "  ACME-Corp  ";

        // Act
        var org = new Organization(name, slug);

        // Assert
        Assert.NotEqual(Guid.Empty, org.Id);
        Assert.Equal("Acme Corporation", org.Name);
        Assert.Equal("acme-corp", org.Slug);
        Assert.True(org.IsActive);
        Assert.True(org.CreatedAtUtc <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string? invalidName)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Organization(invalidName!, "acme"));
        Assert.Contains("Organization name is required.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidSlug_ThrowsArgumentException(string? invalidSlug)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new Organization("Acme", invalidSlug!));
        Assert.Contains("Organization slug is required.", ex.Message);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var org = new Organization("Acme", "acme");

        // Act
        org.Deactivate();

        // Assert
        Assert.False(org.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        // Arrange
        var org = new Organization("Acme", "acme");
        org.Deactivate();

        // Act
        org.Activate();

        // Assert
        Assert.True(org.IsActive);
    }
}
