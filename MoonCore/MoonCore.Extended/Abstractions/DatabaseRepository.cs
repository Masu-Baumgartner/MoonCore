using Microsoft.EntityFrameworkCore;
using MoonCore.Extended.Helpers;

namespace MoonCore.Extended.Abstractions;

public class DatabaseRepository<TEntity> where TEntity : class
{
    private DbSet<TEntity> DbSet;
    private DbContext DataContext;

    public DatabaseRepository(DatabaseHelper databaseHelper, IServiceProvider serviceProvider)
    {
        // Resolve mapping
        var dbContextType = databaseHelper.GetDbContextForEntity<TEntity>();

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

    public TEntity Add(TEntity entity)
    {
        var x = DbSet.Add(entity);
        DataContext.SaveChanges();
        return x.Entity;
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
        DataContext.SaveChanges();
    }
    
    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
        DataContext.SaveChanges();
    }
}