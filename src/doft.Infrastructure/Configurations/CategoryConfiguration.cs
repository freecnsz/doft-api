using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(c => c.Color).HasMaxLength(10);

        // Add relationships
        builder.HasMany(c => c.DoftTasks)
               .WithOne(t => t.Category)
               .HasForeignKey(t => t.CategoryId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Notes)
               .WithOne(n => n.Category)
               .HasForeignKey(n => n.CategoryId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Events)
               .WithOne(e => e.Category)
               .HasForeignKey(e => e.CategoryId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
