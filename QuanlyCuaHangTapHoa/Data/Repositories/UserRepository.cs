using Microsoft.EntityFrameworkCore;
using QuanlyCuaHangTapHoa.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    return null;

                username = username.Trim().ToLower();

                return await _table
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == username);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[UserRepository] GetByUsername error: {ex.Message}");
                return null;
            }
        }
    }
}
