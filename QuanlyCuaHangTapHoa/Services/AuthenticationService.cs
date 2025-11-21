using BCrypt.Net;
using QuanlyCuaHangTapHoa.Data.Repositories;
using QuanlyCuaHangTapHoa.Models;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.Services
{
    /// <summary>
    /// Xử lý logic đăng nhập, đăng xuất, đổi mật khẩu
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepo;
        private const string CURRENT_USER_KEY = "CURRENT_USER_ID";

        public AuthenticationService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // ================================
        // LOGIN
        // ================================
        public async Task<(bool Success, string Message, User? User)> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Vui lòng nhập đầy đủ thông tin.", null);

            var user = await _userRepo.GetByUsernameAsync(username);
            if (user == null)
                return (false, "Tên đăng nhập không tồn tại.", null);

            bool passwordMatch = false;

            // 1. Thử kiểm tra dạng BCrypt (trong trường hợp sau này bạn lưu hash)
            try
            {
                passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch
            {
                // nếu chuỗi trong DB không phải hash hợp lệ thì Verify sẽ ném lỗi → bỏ qua
            }

            // 2. Nếu không khớp, kiểm tra plain text (trường hợp như hiện tại)
            if (!passwordMatch)
            {
                passwordMatch = password == user.PasswordHash;
            }

            if (!passwordMatch)
                return (false, "Mật khẩu không đúng.", null);

            // Lưu user ID vào SecureStorage
            await SecureStorage.SetAsync(CURRENT_USER_KEY, user.Id.ToString());

            return (true, "Đăng nhập thành công.", user);
        }


        // ================================
        // LOGOUT
        // ================================
        public async Task LogoutAsync()
        {
            SecureStorage.Remove(CURRENT_USER_KEY);
            await Task.CompletedTask;
        }

        // ================================
        // KIỂM TRA ĐÃ ĐĂNG NHẬP
        // ================================
        public bool IsLoggedIn()
        {
            // Vì SecureStorage là async → phải gọi Result trong hàm sync
            var task = SecureStorage.GetAsync(CURRENT_USER_KEY);
            task.Wait();

            return !string.IsNullOrEmpty(task.Result);
        }

        // ================================
        // LẤY USER HIỆN TẠI
        // ================================
        public User? GetCurrentUser()
        {
            var task = SecureStorage.GetAsync(CURRENT_USER_KEY);
            task.Wait();

            if (int.TryParse(task.Result, out int userId))
            {
                var userTask = _userRepo.GetByIdAsync(userId);
                userTask.Wait();
                return userTask.Result;
            }

            return null;
        }

        // ================================
        // ĐỔI MẬT KHẨU
        // ================================
        public async Task<(bool Success, string Message)> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            var currentUser = GetCurrentUser();

            if (currentUser == null)
                return (false, "Bạn chưa đăng nhập.");

            bool oldMatch = BCrypt.Net.BCrypt.Verify(oldPassword, currentUser.PasswordHash);
            if (!oldMatch)
                return (false, "Mật khẩu cũ không đúng.");

            // Hash mật khẩu mới
            currentUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            var result = await _userRepo.UpdateAsync(currentUser);

            return result
                ? (true, "Đổi mật khẩu thành công.")
                : (false, "Đổi mật khẩu thất bại.");
        }
    }
}
