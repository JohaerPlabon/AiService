using AiService.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiService.DataManager
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Email).HasMaxLength(200);
                b.Property(x => x.UserName).HasMaxLength(100);
                b.HasIndex(x => x.Email).IsUnique();

                b.Property(x => x.IsEmailVerified).HasDefaultValue(0);
                b.Property(x => x.VerificationToken).HasMaxLength(200).IsRequired(false);
                b.Property(x => x.TokenGeneratedAt).IsRequired(false);
                b.Property(x => x.TokenExpiryTime).IsRequired(false);
                b.Property(x => x.IsSignedIn).IsRequired(true).HasDefaultValue(false);
            });
        }
    }
}
