using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using WebMVC.Utility;

namespace WebMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ResearchProduct> ResearchProducts { get; set; }
        public DbSet<ResearchProductCategory> ResearchProductCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<News> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ResearchProduct)
                .WithMany(rp => rp.Comments)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ApplicationUser)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ResearchProduct>()
                .HasOne(rp => rp.ApplicationUser)
                .WithMany(u => u.ResearchProducts)
                .HasForeignKey(rp => rp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ResearchProductCategory>()
                .HasOne(rpc => rpc.ResearchProduct)
                .WithMany(rp => rp.ResearchProductCategories)
                .HasForeignKey(rpc => rpc.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
