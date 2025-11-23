using QuanlyCuaHangTapHoa.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Services
{
    public interface ISalesService
    {
        Task<Sale> CreateSaleAsync(
            List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountAmount)> items,
            int? customerId,
            int userId,
            decimal receivedAmount,
            string paymentMethod,
            string? notes);

        Task<List<Sale>> GetSalesByDateRangeAsync(DateTime from, DateTime to);
        Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime from, DateTime to);
    }
}
