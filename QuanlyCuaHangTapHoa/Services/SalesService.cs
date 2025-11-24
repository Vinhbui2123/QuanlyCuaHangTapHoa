using QuanlyCuaHangTapHoa.Data;
using QuanlyCuaHangTapHoa.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Services
{
    public class SalesService : ISalesService
    {
        private readonly AppDbContext _db;

        public SalesService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Sale> CreateSaleAsync(
            List<(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountAmount)> items,
            int? customerId,
            int userId,
            decimal receivedAmount,
            string paymentMethod,
            string? notes)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("Hóa đơn không có mặt hàng nào.");

            // KIỂM TRA TỒN KHO TRƯỚC KHI TẠO HÓA ĐƠN
            foreach (var item in items)
            {
                var product = await _db.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                    throw new InvalidOperationException($"Không tìm thấy sản phẩm (Id={item.ProductId}).");

                if (product.StockQuantity < item.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Sản phẩm '{product.Name}' chỉ còn {product.StockQuantity} {product.Unit} trong kho, " +
                        $"không đủ để bán {item.Quantity}.");
                }
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // Tính tổng tiền
                decimal totalAmount = 0;
                decimal totalDiscount = 0;

                foreach (var item in items)
                {
                    totalAmount += item.UnitPrice * item.Quantity;
                    totalDiscount += item.DiscountAmount;
                }

                decimal finalAmount = totalAmount - totalDiscount;
                decimal changeAmount = receivedAmount - finalAmount;
                if (changeAmount < 0) changeAmount = 0;

                // Tạo mã hóa đơn đơn giản
                var todayCount = await _db.Sales.CountAsync(s => s.Date.Date == DateTime.Today);
                string code = $"HD{DateTime.Today:yyyyMMdd}_{todayCount + 1:000}";

                // Tạo đối tượng Sale
                var sale = new Sale
                {
                    Code = code,
                    Date = DateTime.Now,
                    CustomerId = customerId,
                    UserId = userId,
                    TotalAmount = totalAmount,
                    DiscountAmount = totalDiscount,
                    FinalAmount = finalAmount,
                    ReceivedAmount = receivedAmount,
                    ChangeAmount = changeAmount,
                    PaymentMethod = paymentMethod,
                    Notes = notes
                };

                await _db.Sales.AddAsync(sale);
                await _db.SaveChangesAsync(); // để có Id

                // Tạo từng chi tiết + cập nhật tồn kho
                foreach (var item in items)
                {
                    var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product == null)
                        throw new Exception("Không tìm thấy sản phẩm với Id = " + item.ProductId);

                    // Chi tiết hóa đơn
                    var detail = new SaleDetail
                    {
                        SaleId = sale.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        DiscountAmount = item.DiscountAmount,
                        TotalPrice = item.UnitPrice * item.Quantity - item.DiscountAmount
                    };
                    await _db.SaleDetails.AddAsync(detail);

                    // Trừ tồn kho
                    product.StockQuantity -= item.Quantity;
                    if (product.StockQuantity < 0) product.StockQuantity = 0;

                    if (product.StockQuantity == 0) product.Status = "OutOfStock";
                    else if (product.StockQuantity <= 5) product.Status = "LowStock";
                    else product.Status = "InStock";

                    // Ghi lịch sử tồn kho
                    var movement = new StockMovement
                    {
                        ProductId = product.Id,
                        QuantityChange = -item.Quantity,
                        CurrentStockAfter = product.StockQuantity,
                        Type = 1, // 1: OUT
                        ReferenceType = "Sale",
                        ReferenceId = sale.Id,
                        Note = $"Bán hàng - {sale.Code}"
                    };
                    await _db.StockMovements.AddAsync(movement);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return sale;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SalesService] CreateSaleAsync error: {ex.Message}");
                await transaction.RollbackAsync();
                throw; // để ViewModel hiển thị message chung
            }
        }

        public async Task<List<Sale>> GetSalesByDateRangeAsync(DateTime from, DateTime to)
        {
            var toDate = to.Date.AddDays(1);

            return await _db.Sales
                .Where(s => s.Date >= from.Date && s.Date < toDate)
                .OrderByDescending(s => s.Date)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime from, DateTime to)
        {
            var toDate = to.Date.AddDays(1);

            return await _db.Sales
                .Where(s => s.Date >= from.Date && s.Date < toDate)
                .SumAsync(s => s.FinalAmount);
        }
    }
}
