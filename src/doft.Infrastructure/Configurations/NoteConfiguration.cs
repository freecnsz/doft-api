using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("Notes");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Content).IsRequired();

        builder.HasOne(n => n.Owner)
               .WithMany(u => u.Notes)
               .HasForeignKey(n => n.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Category)
               .WithMany(c => c.Notes)
               .HasForeignKey(n => n.CategoryId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
