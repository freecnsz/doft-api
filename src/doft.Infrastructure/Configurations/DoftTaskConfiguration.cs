using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DoftTaskConfiguration : IEntityTypeConfiguration<DoftTask>
{
    public void Configure(EntityTypeBuilder<DoftTask> builder)
    {
        builder.ToTable("DoftTasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Status)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(t => t.Priority)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(t => t.DueDate);

        builder.HasOne(t => t.Owner)
               .WithMany(u => u.DoftTasks)
               .HasForeignKey(t => t.OwnerId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.HasOne(t => t.Category)
               .WithMany(t => t.DoftTasks)
               .HasForeignKey(t => t.CategoryId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
