using QuanlyCuaHangTapHoa.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<(bool Success, string Message)> SaveAsync(Category category);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
