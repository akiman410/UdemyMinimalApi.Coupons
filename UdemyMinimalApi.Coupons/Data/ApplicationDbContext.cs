using Microsoft.EntityFrameworkCore;
using UdemyMinimalApi.Coupons.Models;

namespace UdemyMinimalApi.Coupons.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupon>().HasData(
             new Coupon
             {
                 Id = 1,
                 Name = "10OFF",
                 IsActive = true
             },
            new Coupon
            {
                Id = 2,
                Name = "20OFF",
                IsActive = false,
            });
        }
    }
}
