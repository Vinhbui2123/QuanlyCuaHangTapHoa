using System.Collections.Generic;
using System.Threading.Tasks;
using QuanlyCuaHangTapHoa.Models;

namespace QuanlyCuaHangTapHoa.Data.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        /// <summary>
        /// Tìm kiếm sản phẩm theo tên hoặc mã
        /// </summary>
        Task<List<Product>> SearchByNameOrCodeAsync(string keyword);

        /// <summary>
        /// Lấy 1 sản phẩm theo mã (dùng cho quét mã vạch)
        /// </summary>
        Task<Product?> GetByCodeAsync(string code);
    }
}
