using Microsoft.EntityFrameworkCore;
using MoonCore.Extended.Helpers;

namespace MoonCore.Extended.Abstractions;

/// <summary>
/// Generic repository which retrieves the correct <see cref="DbContext"/> and chooses the correct
/// <see cref="DbSet{TEntity}"/> and makes it available using the repository methods.
/// Requires the <see cref="Extensions.ServiceCollectionExtensions.AddDatabaseMappings"/> and
/// <see cref="Extensions.ServiceCollectionExtensions.AddServiceCollectionAccessor"/> methods being called on the service collection and
/// <see cref="Extensions.ServiceProviderExtensions.GenerateDatabaseMappings"/> being called on the service provider
/// in order to successfully retrieve the mappings
///
/// <code>
/// builder.Services.AddServiceCollectionAccessor();
/// builder.Services.AddServiceCollectionAccessor();
///
/// 
/// var app = builder.Build();
///
/// 
/// app.Services.GenerateDatabaseMappings();
/// </code>
/// </summary>
/// <typeparam name="TEntity"></typeparam>
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

    /// <summary>
    /// Returns access to the db set of the requested entity. Should only be used for querying data.
    /// For raw write access to the DbSet use <see cref="RunTransactionAsync(Func{DbSet{TEntity}, Task}"/>
    /// </summary>
    /// <returns></returns>
    public DbSet<TEntity> Get()
    {
        return DbSet;
    }

    /// <summary>
    /// Adds a new entity and returns the saved version with filled out database generated fields like an auto increment id
    /// </summary>
    /// <param name="entity">Entity to save</param>
    /// <returns>Saved entity</returns>
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var result = await DataContext.AddAsync(entity);
        await DataContext.SaveChangesAsync();

        return result.Entity;
    }

    /// <summary>
    /// Updates the provided entity
    /// </summary>
    /// <param name="entity">Entity to update</param>
    public async Task UpdateAsync(TEntity entity)
    {
        DataContext.Update(entity);
        await DataContext.SaveChangesAsync();
    }

    /// <summary>
    /// Removes the provided entity
    /// </summary>
    /// <param name="entity">Entity to remove</param>
    public async Task RemoveAsync(TEntity entity)
    {
        DataContext.Remove(entity);
        await DataContext.SaveChangesAsync();
    }

    /// <summary>
    /// Runs all in <see cref="transaction"/> defined database calls and the saves the changes in one go.
    /// Useful for large operations like adding a wide range of objects
    /// </summary>
    /// <param name="transaction">Callback providing access to the DbSet</param>
    public async Task RunTransactionAsync(Action<DbSet<TEntity>> transaction)
    {
        transaction.Invoke(DbSet);
        await DataContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Runs all in <see cref="transactionTask"/> defined database calls and the saves the changes in one go.
    /// Useful for large operations like adding a wide range of objects
    /// </summary>
    /// <param name="transactionTask">Callback providing access to the DbSet</param>
    public async Task RunTransactionAsync(Func<DbSet<TEntity>, Task> transactionTask)
    {
        await transactionTask.Invoke(DbSet);
        await DataContext.SaveChangesAsync();
    }
}