using QuanlyCuaHangTapHoa.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> SearchAsync(string keyword);
        Task<(bool Success, string Message)> SaveAsync(Product product);  // thêm / sửa
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<List<Product>> GetLowStockProductsAsync(int threshold = 5);
        Task<(bool Success, string Message)> AdjustStockAsync(int productId, int changeAmount, string reason);
    }
}
