using FreelanceBirga.Controllers;
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
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrdersChat> OrdersChat { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ReviewCustomer> ReviewsCustomer { get; set; }
    public DbSet<ReviewExecutor> ReviewsExecutor { get; set; }
    public DbSet<OrderChatForRewiew> OrdersChatForRewiew { get; set; }
}