using QuanlyCuaHangTapHoa.Data;
using QuanlyCuaHangTapHoa.Data.Repositories;
using QuanlyCuaHangTapHoa.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Services
{
    /// <summary>
    /// Xử lý nghiệp vụ liên quan đến hàng hóa
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly AppDbContext _db;

        public ProductService(IProductRepository productRepo, AppDbContext db)
        {
            _productRepo = productRepo;
            _db = db;
        }

        public Task<List<Product>> GetAllAsync()
            => _productRepo.GetAllAsync();

        public Task<Product?> GetByIdAsync(int id)
            => _productRepo.GetByIdAsync(id);

        public Task<List<Product>> SearchAsync(string keyword)
            => _productRepo.SearchByNameOrCodeAsync(keyword);

        public async Task<(bool Success, string Message)> SaveAsync(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                return (false, "Tên sản phẩm không được để trống.");

            if (string.IsNullOrWhiteSpace(product.Code))
                return (false, "Mã sản phẩm không được để trống.");

            try
            {
                // Kiểm tra trùng mã khi thêm mới hoặc sửa
                var existed = await _productRepo.FindAsync(p => p.Code == product.Code && p.Id != product.Id);
                if (existed.Any())
                    return (false, "Mã sản phẩm đã tồn tại.");

                if (product.Id == 0)
                    await _productRepo.AddAsync(product);
                else
                    await _productRepo.UpdateAsync(product);

                return (true, "Lưu sản phẩm thành công.");
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[ProductService] SaveAsync error: {ex.Message}");
                return (false, "Lưu sản phẩm thất bại.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var ok = await _productRepo.DeleteAsync(id);
                return ok
                    ? (true, "Xóa sản phẩm thành công.")
                    : (false, "Không tìm thấy sản phẩm để xóa.");
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[ProductService] DeleteAsync error: {ex.Message}");
                return (false, "Xóa sản phẩm thất bại.");
            }
        }

        public async Task<List<Product>> GetLowStockProductsAsync(int threshold = 5)
        {
            try
            {
                return await _db.Products
                    .Where(p => p.StockQuantity <= threshold && p.IsActive)
                    .OrderBy(p => p.StockQuantity)
                    .ToListAsync();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[ProductService] GetLowStockProductsAsync error: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<(bool Success, string Message)> AdjustStockAsync(int productId, int changeAmount, string reason)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
                return (false, "Không tìm thấy sản phẩm.");

            product.StockQuantity += changeAmount;
            if (product.StockQuantity < 0)
                product.StockQuantity = 0;

            // Cập nhật trạng thái tồn kho đơn giản
            if (product.StockQuantity == 0) product.Status = "OutOfStock";
            else if (product.StockQuantity <= 5) product.Status = "LowStock";
            else product.Status = "InStock";

            try
            {
                // Cập nhật sản phẩm
                await _productRepo.UpdateAsync(product);

                // Ghi log tồn kho
                var movement = new StockMovement
                {
                    ProductId = product.Id,
                    QuantityChange = changeAmount,
                    CurrentStockAfter = product.StockQuantity,
                    Type = changeAmount >= 0 ? 0 : 1, // 0: IN, 1: OUT
                    ReferenceType = "Adjustment",
                    Note = reason
                };

                _db.StockMovements.Add(movement);
                await _db.SaveChangesAsync();

                return (true, "Điều chỉnh tồn kho thành công.");
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[ProductService] AdjustStockAsync error: {ex.Message}");
                return (false, "Điều chỉnh tồn kho thất bại.");
            }
        }
    }
}
