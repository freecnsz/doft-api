using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AttachmentLinkConfiguration : IEntityTypeConfiguration<AttachmentLink>
{
    public void Configure(EntityTypeBuilder<AttachmentLink> builder)
    {
        builder.ToTable("AttachmentLinks");

        builder.HasKey(al => al.Id);

        builder.Property(al => al.ItemType)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();

        builder.HasOne(al => al.Attachment)
               .WithMany(a => a.AttachmentLinks)
               .HasForeignKey(al => al.AttachmentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
