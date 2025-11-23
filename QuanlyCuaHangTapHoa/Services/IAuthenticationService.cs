using QuanlyCuaHangTapHoa.Models;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Services
{
    public interface IAuthenticationService
    {
        Task<(bool Success, string Message, User? User)> LoginAsync(string username, string password);
        Task LogoutAsync();
        Task<(bool Success, string Message)> ChangePasswordAsync(string oldPassword, string newPassword);

        // ✅ Dùng cho UI: phiên bản async
        Task<User?> GetCurrentUserAsync();
        Task<bool> IsLoggedInAsync();

        // Giữ lại cho nơi nào cần sync (hạn chế dùng)
        User? GetCurrentUser();
        bool IsLoggedIn();

        Task<(bool Success, string Message)> RegisterAsync(
            string username,
            string password,
            string fullName,
            string? phone,
            string? email);
    }
}
