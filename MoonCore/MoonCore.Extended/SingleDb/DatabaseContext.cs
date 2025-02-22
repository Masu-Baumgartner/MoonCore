using Microsoft.EntityFrameworkCore;

namespace MoonCore.Extended.SingleDb;

public abstract class DatabaseContext : DbContext
{
    private readonly DatabaseOptions Options;

    public abstract string Prefix { get; }

    public DatabaseContext(DatabaseOptions options)
    {
        Options = options;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        var connectionString = $"Host={Options.Host};" +
                               $"Port={Options.Port};" +
                               $"Database={Options.Database};" +
                               $"Username={Options.Username};" +
                               $"Password={Options.Password}";

        optionsBuilder.UseNpgsql(
            connectionString,
            builder =>
            {
                builder.EnableRetryOnFailure(5);
                builder.MigrationsHistoryTable($"{Prefix}_MigrationHistory");
            }
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var prefixStr = $"{Prefix}_";

        foreach (var entity in (modelBuilder.Model.GetEntityTypes()))
        {
            if (!entity.GetTableName()?.StartsWith(prefixStr) ?? false)
            {
                modelBuilder
                    .Entity(entity.ClrType)
                    .ToTable(prefixStr + entity.GetTableName());
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}