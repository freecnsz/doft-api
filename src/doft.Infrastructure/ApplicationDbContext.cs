using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using doft.Domain.Entities;
using doft.Domain.Enums;

namespace doft.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppUserRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define all DbSets
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<DoftTask> DoftTasks { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Detail> Details { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagLink> TagLinks { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<AttachmentLink> AttachmentLinks { get; set; }
        public DbSet<Preference> Preferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations from the Configurations folder
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
