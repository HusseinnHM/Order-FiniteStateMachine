using Microsoft.EntityFrameworkCore;
using OrderSample.API.Entities;

namespace OrderSample.API.DbContexts;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> context) : base(context) { }

    public DbSet<Order> Orders => Set<Order>();

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().OwnsMany(o => o.OrderStages);
        base.OnModelCreating(modelBuilder);
    }

}
