using Microsoft.EntityFrameworkCore;
using MoonCore.Abstractions;
using MoonCore.Blazor.Test.Database;

namespace MoonCore.Blazor.Test.Services;

public class GenericRepository<TEntity> : Repository<TEntity> where TEntity : class
{
    private readonly SomeContext DataContext;
    private readonly DbSet<TEntity> DbSet;

    public GenericRepository(SomeContext dbContext)
    {
        DataContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        DbSet = DataContext.Set<TEntity>();
    }

    public override DbSet<TEntity> Get()
    {
        return DbSet;
    }

    public override TEntity Add(TEntity entity)
    {
        var x = DbSet.Add(entity);
        DataContext.SaveChanges();
        return x.Entity;
    }

    public override void Update(TEntity entity)
    {
        DbSet.Update(entity);
        DataContext.SaveChanges();
    }
    
    public override void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
        DataContext.SaveChanges();
    }
}