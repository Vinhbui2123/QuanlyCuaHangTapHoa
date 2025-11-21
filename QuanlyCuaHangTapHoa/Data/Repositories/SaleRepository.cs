using Microsoft.EntityFrameworkCore;
using QuanlyCuaHangTapHoa.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Data.Repositories
{
    public class SaleRepository : Repository<Sale>, ISaleRepository
    {
        public SaleRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<List<Sale>> GetSalesByDateRangeAsync(DateTime from, DateTime to)
        {
            try
            {
                // đảm bảo "to" bao trọn ngày
                var toDate = to.Date.AddDays(1);

                return await _table
                    .Where(s => s.Date >= from.Date && s.Date < toDate)
                    .OrderByDescending(s => s.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SaleRepository] GetSalesByDateRange error: {ex.Message}");
                return new List<Sale>();
            }
        }

        public async Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime from, DateTime to)
        {
            try
            {
                var toDate = to.Date.AddDays(1);

                return await _table
                    .Where(s => s.Date >= from.Date && s.Date < toDate)
                    .SumAsync(s => s.FinalAmount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SaleRepository] GetTotalRevenueByDateRange error: {ex.Message}");
                return 0m;
            }
        }
    }
}
