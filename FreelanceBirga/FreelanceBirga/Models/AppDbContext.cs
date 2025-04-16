using FreelanceBirga.Models;
using FreelanceBirga.Models.DB;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Executor> Executors { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ExecutorTag> ExecutorsTag { get; set; }
    public DbSet<TempOrder> TempOrders { get; set; }
}