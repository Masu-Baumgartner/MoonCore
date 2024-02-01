using Microsoft.EntityFrameworkCore;

namespace MoonCore.Abstractions;

public abstract class Repository<TEntity> where TEntity : class
{
    public abstract DbSet<TEntity> Get();

    public abstract TEntity Add(TEntity entity);

    public abstract void Update(TEntity entity);

    public abstract void Delete(TEntity entity);
}