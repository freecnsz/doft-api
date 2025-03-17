using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DetailConfiguration : IEntityTypeConfiguration<Detail>
{
    public void Configure(EntityTypeBuilder<Detail> builder)
    {
        builder.ToTable("Details");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.ItemType)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(d => d.Title);
        builder.Property(d => d.Description);
        builder.Property(d => d.HasAttachment);
        builder.Property(d => d.HasTag);
        builder.Property(d => d.IsDeleted);
        builder.Property(d => d.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(d => d.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

    }
}
