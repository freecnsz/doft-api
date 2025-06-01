using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PlannedTaskConfiguration : IEntityTypeConfiguration<PlannedTask>
{
    public void Configure(EntityTypeBuilder<PlannedTask> builder)
    {
        builder.ToTable("PlannedTasks");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PlannedDate)
               .IsRequired();

        builder.Property(p => p.StartTime)
               .IsRequired();

        builder.Property(p => p.Duration)
               .IsRequired();

        builder.HasOne(p => p.DoftTask)
               .WithOne()
               .HasForeignKey<PlannedTask>(p => p.TaskId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
