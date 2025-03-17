using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.ToTable("Reminders");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReminderPeriod)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(r => r.ReminderTime);

        builder.HasOne(r => r.DoftTask)
               .WithMany(t => t.Reminders)
               .HasForeignKey(r => r.TaskId);

        builder.HasOne(r => r.User)
               .WithMany(u => u.Reminders)
               .HasForeignKey(r => r.UserId);
    }
}
