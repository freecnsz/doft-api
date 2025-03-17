using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
{
    public void Configure(EntityTypeBuilder<Preference> builder)
    {
        builder.ToTable("Preferences");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Language).HasMaxLength(10);
        builder.Property(p => p.Theme).HasMaxLength(20);
        builder.Property(p => p.NotificationEnabled);
        builder.Property(p => p.EmailNotifications);
        builder.Property(p => p.PushNotifications);
        builder.Property(p => p.TimeZone).HasMaxLength(50);
        builder.Property(p => p.DateFormat).HasMaxLength(20);
        builder.Property(p => p.TimeFormat).HasMaxLength(10);

        builder.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(p => p.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(p => p.User)
               .WithMany(u => u.Preferences)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
