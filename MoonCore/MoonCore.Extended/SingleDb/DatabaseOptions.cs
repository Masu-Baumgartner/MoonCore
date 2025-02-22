using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace MoonCore.Extended.SingleDb;

public class DatabaseOptions
{
    public string Host { get; set; }
    public int Port { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }

    public string Database { get; set; }

    public Action<NpgsqlDbContextOptionsBuilder>? ConfigureDbContext { get; set; }
}