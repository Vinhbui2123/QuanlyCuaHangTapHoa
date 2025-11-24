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

        public async Task<List<Product>> GetAllAsync()  
        {
            // Chỉ lấy sản phẩm còn hoạt động
            return await _db.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }


        public Task<Product?> GetByIdAsync(int id)
            => _productRepo.GetByIdAsync(id);

        public Task<List<Product>> SearchAsync(string keyword)
            => _productRepo.SearchByNameOrCodeAsync(keyword);

        /// <summary>
        /// Thêm mới hoặc cập nhật sản phẩm
        /// </summary>
        public async Task<(bool Success, string Message)> SaveAsync(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                return (false, "Tên sản phẩm không được để trống.");

            if (string.IsNullOrWhiteSpace(product.Code))
                return (false, "Mã sản phẩm không được để trống.");

            try
            {
                // kiểm tra trùng mã (trừ chính nó)
                var existed = await _db.Products
                    .Where(p => p.Code == product.Code && p.Id != product.Id)
                    .ToListAsync();

                if (existed.Any())
                    return (false, "Mã sản phẩm đã tồn tại.");

                if (product.Id == 0)
                {
                    // THÊM MỚI
                    await _db.Products.AddAsync(product);
                }
                else
                {
                    // CẬP NHẬT: lấy entity trong DB rồi gán lại thuộc tính
                    var dbProduct = await _db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
                    if (dbProduct == null)
                        return (false, "Không tìm thấy sản phẩm để cập nhật.");

                    dbProduct.Code = product.Code;
                    dbProduct.Name = product.Name;
                    dbProduct.CategoryId = product.CategoryId;
                    dbProduct.Unit = product.Unit;
                    dbProduct.PurchasePrice = product.PurchasePrice;
                    dbProduct.SellingPrice = product.SellingPrice;
                    dbProduct.StockQuantity = product.StockQuantity;
                    dbProduct.Status = product.Status; 
                    dbProduct.IsActive = product.IsActive;

                    // ⬇️ TÍNH LẠI STATUS Ở ĐÂY
                    if (dbProduct.StockQuantity == 0)
                        dbProduct.Status = "OutOfStock";
                    else if (dbProduct.StockQuantity <= 5)
                        dbProduct.Status = "LowStock";
                    else
                        dbProduct.Status = "InStock";
                }

                await _db.SaveChangesAsync();
                return (true, "Lưu sản phẩm thành công.");
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[ProductService] SaveAsync error: {ex.Message}");
                return (false, "Lưu sản phẩm thất bại.");
            }
        }

        /// <summary>
        /// Xóa sản phẩm theo Id
        /// </summary>
        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                    return (false, "Không tìm thấy sản phẩm để xóa.");

                // Kiểm tra đã phát sinh giao dịch chưa
                bool hasSaleDetails = await _db.SaleDetails.AnyAsync(d => d.ProductId == id);
                bool hasMovements = await _db.StockMovements.AnyAsync(m => m.ProductId == id);

                if (hasSaleDetails || hasMovements)
                {
                    // ❗ Không xóa cứng, chỉ khóa sản phẩm
                    product.IsActive = false;
                    await _db.SaveChangesAsync();
                    return (true, "Sản phẩm đã phát sinh giao dịch, hệ thống sẽ khóa sản phẩm (không xóa hẳn).");
                }

                // Chưa có giao dịch nào → cho xóa cứng
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();

                return (true, "Xóa sản phẩm thành công.");
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
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
                return (false, "Không tìm thấy sản phẩm.");

            product.StockQuantity += changeAmount;
            if (product.StockQuantity < 0)
                product.StockQuantity = 0;

            if (product.StockQuantity == 0) product.Status = "OutOfStock";
            else if (product.StockQuantity <= 5) product.Status = "LowStock";
            else product.Status = "InStock";

            try
            {
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
