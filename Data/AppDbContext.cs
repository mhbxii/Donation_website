using dotnet9.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Suppress the warning about dynamic default values used in HasData calls.
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }*/

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleImage> ArticleImages { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Identity tables first.
            base.OnModelCreating(modelBuilder);

            //Configure Default Values (GUID):
            modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<Category>()
            .Property(c => c.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Article>()
            .Property(a => a.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<ContactUs>()
            .Property(cu => cu.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<ArticleImage>()
            .Property(ai => ai.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();
            
            //CreatedAt
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Article>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Article>()
                .Property(a => a.UpdatedAt)
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<ContactUs>()
                .Property(cu => cu.CreatedAt)
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd();


            //Relations:
            modelBuilder.Entity<User>()
                .HasMany(u => u.Articles)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        
            modelBuilder.Entity<User>()
                .HasMany(u => u.ContactUs)
                .WithOne(cu => cu.User)
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Article>()
                .HasMany(a => a.ArticleImages)
                .WithOne(i => i.Article)
                .HasForeignKey(i => i.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Articles)
                .WithOne(a => a.Category)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);


            // Seed roles with raw GUID keys.
            var adminRoleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var userRoleId = Guid.Parse("00000000-0000-0000-0000-000000000002");
            List<IdentityRole<Guid>> roles = new List<IdentityRole<Guid>>
            {
                new IdentityRole<Guid>
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<Guid>
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                }
            };
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(roles);
            
        }
    }
}