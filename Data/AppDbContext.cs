using dotnet9.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Article> Articles { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Identity tables first.
            base.OnModelCreating(modelBuilder);

            // Configure default values (GUID generation)
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

            modelBuilder.Entity<Request>()
                .Property(r => r.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ContactUs>()
                .Property(cu => cu.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Image>()
                .Property(i => i.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            // Set CreatedAt defaults
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

            modelBuilder.Entity<Request>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Request>()
                .Property(r => r.UpdatedAt)
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ContactUs>()
                .Property(cu => cu.CreatedAt)
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd();

            // Relationships:
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
                .HasMany(a => a.Requests)
                .WithOne(r => r.Article)
                .HasForeignKey(r => r.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Articles)
                .WithOne(a => a.Category)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure index for polymorphic image lookup.
            modelBuilder.Entity<Image>()
                .HasIndex(i => i.ParentId);

            // Seed roles (unchanged)
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

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Food" },
                new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Meubles" },
                new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Clothes" }
            );

        }
    }
}
