using FinPilot.Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinPilot.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.Slug)
            .IsUnique();

        builder.Property(e => e.IsActive)
            .IsRequired();

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired();
    }
}
