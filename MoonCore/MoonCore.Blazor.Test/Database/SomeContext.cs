using Microsoft.EntityFrameworkCore;

namespace MoonCore.Blazor.Test.Database;

public class SomeContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(optionsBuilder.IsConfigured)
            return;

        optionsBuilder.UseSqlite("Data Source=data.sqlite");
    }
}