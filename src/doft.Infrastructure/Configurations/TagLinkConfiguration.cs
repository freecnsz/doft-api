using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TagLinkConfiguration : IEntityTypeConfiguration<TagLink>
{
       public void Configure(EntityTypeBuilder<TagLink> builder)
       {
              builder.ToTable("TagLinks");

              builder.HasKey(t => t.Id);

              builder.Property(t => t.ItemType)
                     .HasConversion<string>() // Saves enum as string
                     .HasMaxLength(50)
                     .IsRequired();

              builder.Property(t => t.ItemId)
                     .IsRequired();

              builder.HasOne(t => t.Tag)
                     .WithMany(t => t.TagLinks)
                     .HasForeignKey(t => t.TagId)
                     .OnDelete(DeleteBehavior.Cascade);

              // Unique constraint to avoid duplicate tags on same item
              builder.HasIndex(t => new { t.ItemType, t.ItemId, t.TagId })
                     .IsUnique();

       }
}

