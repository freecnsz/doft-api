using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FromDate).IsRequired();
        builder.Property(e => e.ToDate).IsRequired();
        builder.Property(e => e.Location).HasMaxLength(255);
        builder.Property(e => e.IsWholeDay);

        builder.HasOne(e => e.Owner)
               .WithMany(u => u.Events)
               .HasForeignKey(e => e.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Category)
               .WithMany(c => c.Events)
               .HasForeignKey(e => e.CategoryId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
