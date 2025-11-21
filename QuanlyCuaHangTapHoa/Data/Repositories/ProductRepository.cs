using Microsoft.EntityFrameworkCore;
using QuanlyCuaHangTapHoa.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Data.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<List<Product>> SearchByNameOrCodeAsync(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return await _table.ToListAsync();

                keyword = keyword.Trim().ToLower();

                return await _table
                    .Where(p =>
                        p.Name.ToLower().Contains(keyword) ||
                        p.Code.ToLower().Contains(keyword))
                    .ToListAsync();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[ProductRepository] Search error: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return null;

                code = code.Trim().ToLower();

                return await _table
                    .FirstOrDefaultAsync(p => p.Code.ToLower() == code);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[ProductRepository] GetByCode error: {ex.Message}");
                return null;
            }
        }
    }
}
