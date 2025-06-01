using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName).HasMaxLength(255);
        builder.Property(a => a.FileType).HasMaxLength(50);
        builder.Property(a => a.FileSize);
        builder.Property(a => a.FilePath);
        builder.Property(a => a.UploadedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(a => a.Owner)
               .WithMany(u => u.Attachments)
               .HasForeignKey(a => a.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);

        // Add relationship with AttachmentLinks
        builder.HasMany(a => a.AttachmentLinks)
               .WithOne(al => al.Attachment)
               .HasForeignKey(al => al.AttachmentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
