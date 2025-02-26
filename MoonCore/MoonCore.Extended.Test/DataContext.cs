using Microsoft.EntityFrameworkCore;
using MoonCore.Extended.SingleDb;

namespace MoonCore.Extended.Test;

public class DataContext : DatabaseContext
{
    public override string Prefix { get; } = "Data";

    public DbSet<Car> Cars { get; set; }

    public DataContext(DatabaseOptions options)
    {
        Options = options;
    }
}