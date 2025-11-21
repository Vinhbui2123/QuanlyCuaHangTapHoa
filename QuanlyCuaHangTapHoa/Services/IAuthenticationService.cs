using QuanlyCuaHangTapHoa.Models;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Services
{
    public interface IAuthenticationService
    {
        Task<(bool Success, string Message, User? User)> LoginAsync(string username, string password);
        Task LogoutAsync();
        Task<(bool Success, string Message)> ChangePasswordAsync(string oldPassword, string newPassword);
        User? GetCurrentUser();
        bool IsLoggedIn();
    }
}
    