using Microsoft.EntityFrameworkCore;

namespace UserChecker.Server.Model;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}