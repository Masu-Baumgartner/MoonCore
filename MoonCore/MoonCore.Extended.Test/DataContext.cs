using Microsoft.EntityFrameworkCore;

namespace MoonCore.Extended.Test;

public class DataContext : DbContext
{
    public DbSet<Car> Cars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(optionsBuilder.IsConfigured)
            return;
        
        var connectionString = "Host=localhost;" +
                               "Port=5432;" +
                               "Username=test_db;" +
                               "Password=test_db;" +
                               "Database=test_db";

        optionsBuilder.UseNpgsql(connectionString);
    }
}