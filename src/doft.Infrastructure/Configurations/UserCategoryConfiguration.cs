using doft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace doft.Infrastructure.Configurations
{
    public class UserCategoryConfiguration : IEntityTypeConfiguration<UserCategory>
    {
        public void Configure(EntityTypeBuilder<UserCategory> builder)
        {
            builder.ToTable("UserCategories");

            builder.HasKey(uc => uc.Id);

            builder.Property(uc => uc.UserId)
                   .IsRequired();

            builder.Property(uc => uc.CategoryId)
                   .IsRequired();


            builder.HasOne(uc => uc.Category)
                   .WithMany(c => c.UserCategories)
                   .HasForeignKey(uc => uc.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Add unique constraint for UserId and CategoryId combination
            builder.HasIndex(uc => new { uc.UserId, uc.CategoryId })
                   .IsUnique();
        }
    }
} 