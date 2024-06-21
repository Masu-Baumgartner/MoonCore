using Microsoft.EntityFrameworkCore;
using MoonCore.Blazor.Test.Data;

namespace MoonCore.Blazor.Test.Database;

public class SomeContext : DbContext
{
    public DbSet<Car> Cars { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(optionsBuilder.IsConfigured)
            return;

        optionsBuilder.UseSqlite("Data Source=data.sqlite");
    }
}