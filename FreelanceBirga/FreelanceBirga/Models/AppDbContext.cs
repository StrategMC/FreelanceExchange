using FreelanceBirga.Models;
using FreelanceBirga.Models.DB;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Executor> Executors { get; set; }
}