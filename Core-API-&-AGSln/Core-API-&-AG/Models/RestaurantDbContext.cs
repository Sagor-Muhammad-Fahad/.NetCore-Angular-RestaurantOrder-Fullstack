using Microsoft.EntityFrameworkCore;

namespace Core_API___AG.Models
{
    public class RestaurantDbContext :DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
        {
        }

        public DbSet<RestaurantTable> RestaurantTables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // RestaurantTable → Order (1 to Many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.RestaurantTable)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.RestaurantTableId)
                .OnDelete(DeleteBehavior.Restrict);  

            // Order → OrderItem (1 to Many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);   

            // MenuItem → OrderItem (1 to Many)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);   

        
        }
    }
}


