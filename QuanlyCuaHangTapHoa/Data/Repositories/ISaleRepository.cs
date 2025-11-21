using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuanlyCuaHangTapHoa.Models;

namespace QuanlyCuaHangTapHoa.Data.Repositories
{
    public interface ISaleRepository : IRepository<Sale>
    {
        /// <summary>
        /// Lấy danh sách hóa đơn theo khoảng ngày
        /// </summary>
        Task<List<Sale>> GetSalesByDateRangeAsync(DateTime from, DateTime to);

        /// <summary>
        /// Tính tổng doanh thu trong khoảng ngày
        /// </summary>
        Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime from, DateTime to);
    }
}
