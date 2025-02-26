using Microsoft.EntityFrameworkCore;
using MoonCore.Extended.Helpers;

namespace MoonCore.Extended.Abstractions;

public class DatabaseRepository<TEntity> where TEntity : class
{
    private DbSet<TEntity> DbSet;
    private DbContext DataContext;

    public DatabaseRepository(DatabaseMappingOptions options, IServiceProvider serviceProvider)
    {
        // Resolve mapping
        var dbContextType = options.Mappings.GetValueOrDefault(typeof(TEntity));

        if (dbContextType == null)
            throw new ArgumentException("The database mapping for this type does not exist", nameof(TEntity));
        
        // Resolve db context
        var dbContext = serviceProvider.GetService(dbContextType) as DbContext;
        
        if(dbContext == null)
            throw new ArgumentException($"The database {dbContextType.FullName} could not be resolved via the dependency injection");
        
        DbSet = dbContext.Set<TEntity>();
        DataContext = dbContext;
    }

    public DbSet<TEntity> Get()
    {
        return DbSet;
    }

    public async Task<TEntity> Add(TEntity entity)
    {
        var result = await DataContext.AddAsync(entity);
        await DataContext.SaveChangesAsync();

        return result.Entity;
    }

    public async Task Update(TEntity entity)
    {
        DataContext.Update(entity);
        await DataContext.SaveChangesAsync();
    }

    public async Task Remove(TEntity entity)
    {
        DataContext.Remove(entity);
        await DataContext.SaveChangesAsync();
    }

    public async Task RunTransaction(Action<DbSet<TEntity>> transaction)
    {
        transaction.Invoke(DbSet);
        await DataContext.SaveChangesAsync();
    }
    
    public async Task RunTransaction(Func<DbSet<TEntity>, Task> transactionTask)
    {
        await transactionTask.Invoke(DbSet);
        await DataContext.SaveChangesAsync();
    }
}