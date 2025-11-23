using Microsoft.EntityFrameworkCore;
using QuanlyCuaHangTapHoa.Data;
using QuanlyCuaHangTapHoa.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;

        public CategoryService(AppDbContext db)
        {
            _db = db;
        }

        public Task<List<Category>> GetAllAsync()
            => _db.Categories.OrderBy(c => c.Name).ToListAsync();

        public Task<Category?> GetByIdAsync(int id)
            => _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<(bool Success, string Message)> SaveAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return (false, "Tên danh mục không được để trống.");

            try
            {
                // kiểm tra trùng tên
                var existed = await _db.Categories
                    .Where(c => c.Name == category.Name && c.Id != category.Id)
                    .ToListAsync();

                if (existed.Any())
                    return (false, "Tên danh mục đã tồn tại.");

                if (category.Id == 0)
                {
                    await _db.Categories.AddAsync(category);
                }
                else
                {
                    var dbCategory = await _db.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
                    if (dbCategory == null)
                        return (false, "Không tìm thấy danh mục để cập nhật.");

                    dbCategory.Name = category.Name;
                    dbCategory.IsActive = category.IsActive;
                }

                await _db.SaveChangesAsync();
                return (true, "Lưu danh mục thành công.");
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[CategoryService] SaveAsync error: {ex.Message}");
                return (false, "Lưu danh mục thất bại.");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (category == null)
                    return (false, "Không tìm thấy danh mục để xóa.");

                // TODO: có thể kiểm tra xem có Product nào đang dùng Category này không

                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
                return (true, "Xóa danh mục thành công.");
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[CategoryService] DeleteAsync error: {ex.Message}");
                return (false, "Xóa danh mục thất bại.");
            }
        }
    }
}
    