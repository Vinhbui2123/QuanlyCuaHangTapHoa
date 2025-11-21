using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Data.Repositories
{
    /// <summary>
    /// Repository chung cho CRUD cơ bản
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _table;

        public Repository(AppDbContext db)
        {
            _db = db;
            _table = _db.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            try
            {
                return await _table.ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Repository] GetAllAsync error: {ex.Message}");
                return new List<T>();
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _table.FindAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Repository] GetByIdAsync error: {ex.Message}");
                return null;
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                await _table.AddAsync(entity);
                await _db.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Repository] AddAsync error: {ex.Message}");
                return entity;
            }
        }

        public async Task<bool> UpdateAsync(T entity)   
        {
            try
            {
                _table.Update(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Repository] UpdateAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _table.FindAsync(id);
                if (entity == null) return false;

                _table.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Repository] DeleteAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _table.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Repository] FindAsync error: {ex.Message}");
                return new List<T>();
            }
        }

        public async Task<int> CountAsync()
        {
            try
            {
                return await _table.CountAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Repository] CountAsync error: {ex.Message}");
                return 0;
            }
        }
    }
}
