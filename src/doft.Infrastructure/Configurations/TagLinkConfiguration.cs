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
               .HasConversion<string>()
               .IsRequired();

        builder.HasOne(t => t.Tag)
                .WithMany()
                .HasForeignKey(t => t.TagId);
    }
}
