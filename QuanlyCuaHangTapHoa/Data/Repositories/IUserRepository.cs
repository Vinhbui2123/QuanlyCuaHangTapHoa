using System.Threading.Tasks;
using QuanlyCuaHangTapHoa.Models;

namespace QuanlyCuaHangTapHoa.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Lấy user theo username (dùng cho đăng nhập)
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);
    }
}
