using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(t => t.Name).IsUnique();

        // Configure relationship with TagLinks
        builder.HasMany(t => t.TagLinks)
               .WithOne(tl => tl.Tag)
               .HasForeignKey(tl => tl.TagId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
