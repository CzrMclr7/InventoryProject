using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace InventoryProject.DataAccess.Services;

public class EfCoreHelper<T>(DbContext context) where T : class
{
    private readonly DbContext _context = context;

    #region Read

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<T?> GetByNameAsync(string name)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(entity => EF.Property<string>(entity, "Name") == name);
    }

    public async Task<T?> GetByCodeAsync(string code)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(entity => EF.Property<string>(entity, "Code") == code);
    }

    public async Task<T?> GetByDescriptionAsync(string description)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(entity => EF.Property<string>(entity, "Description") == description);
    }

    #endregion Read

    #region Create, Update, Delete

    #region With Rollback

    public async Task<T> CreateAsync(T entity, params string[] excludedProperties)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            _context.Set<T>().Add(entity);

            foreach (var excludedProperty in excludedProperties)
            {
                _context.Entry(entity).Property(excludedProperty).IsModified = false;
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            _context.Entry(entity).State = EntityState.Detached;

            return entity;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<T> UpdateAsync(T entity, params string[] excludedProperties)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            _context.Entry(entity).State = EntityState.Modified;

            foreach (var excludedProperty in excludedProperties)
            {
                _context.Entry(entity).Property(excludedProperty).IsModified = false;
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            _context.Entry(entity).State = EntityState.Detached;

            return entity;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(T entity)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task BatchDeleteAsync(IEnumerable<T> entities)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    #endregion With Rollback

    #region Without Rollback

    public async Task<T> CreateNoRollbackAsync(T entity, params string[] excludedProperties)
    {
        try
        {
            _context.Set<T>().Add(entity);

            foreach (var excludedProperty in excludedProperties)
            {
                _context.Entry(entity).Property(excludedProperty).IsModified = false;
            }

            await _context.SaveChangesAsync();

            _context.Entry(entity).State = EntityState.Detached;

            return entity;
        }
        catch
        {
            throw;
        }
    }

    public async Task<T> UpdateNoRollbackAsync(T entity, params string[] excludedProperties)
    {
        try
        {
            _context.Entry(entity).State = EntityState.Modified;

            foreach (var excludedProperty in excludedProperties)
            {
                _context.Entry(entity).Property(excludedProperty).IsModified = false;
            }

            await _context.SaveChangesAsync();

            _context.Entry(entity).State = EntityState.Detached;

            return entity;
        }
        catch
        {
            throw;
        }
    }

    public async Task DeleteNoRollbackAsync(T entity)
    {
        try
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task BatchDeleteNoRollbackAsync(IEnumerable<T> entities)
    {
        try
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw;
        }
    }

    #endregion Without Rollback

    #region Bulk Operations

    #region With Rollback

    public async Task<IEnumerable<T>> BulkInsertAsync(IEnumerable<T> entities, List<string> excludedProperties, int batchSize = 5000)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var bulkConfig = new BulkConfig { BatchSize = batchSize };

            if (excludedProperties != null)
            {
                bulkConfig.PropertiesToExclude = excludedProperties;
            }

            await _context.BulkInsertAsync(entities.ToList(), bulkConfig);

            await transaction.CommitAsync();

            return entities;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<T>> BulkUpdateAsync(IEnumerable<T> entities, List<string> excludedProperties, int batchSize = 5000)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var bulkConfig = new BulkConfig { BatchSize = batchSize };

            if (excludedProperties != null)
            {
                bulkConfig.PropertiesToExclude = excludedProperties;
            }

            await _context.BulkUpdateAsync(entities.ToList(), bulkConfig);

            await transaction.CommitAsync();

            return entities;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    #endregion With Rollback

    #region Without Rollback

    public async Task<IEnumerable<T>> BulkInsertNoRollbackAsync(IEnumerable<T> entities, List<string> excludedProperties, int batchSize = 5000)
    {
        try
        {
            var bulkConfig = new BulkConfig { BatchSize = batchSize };

            if (excludedProperties != null)
            {
                bulkConfig.PropertiesToExclude = excludedProperties;
            }

            await _context.BulkInsertAsync(entities.ToList(), bulkConfig);

            return entities;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IEnumerable<T>> BulkUpdateNoRollbackAsync(IEnumerable<T> entities, List<string> excludedProperties, int batchSize = 5000)
    {
        try
        {
            var bulkConfig = new BulkConfig { BatchSize = batchSize };

            if (excludedProperties != null)
            {
                bulkConfig.PropertiesToExclude = excludedProperties;
            }

            await _context.BulkUpdateAsync(entities.ToList(), bulkConfig);

            return entities;
        }
        catch
        {
            throw;
        }
    }

    #endregion Without Rollback

    #endregion Bulk Operations

    #endregion Create, Update, Delete
}