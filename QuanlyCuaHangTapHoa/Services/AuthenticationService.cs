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
        // ✅ Phiên bản async an toàn cho UI
        public async Task<User?> GetCurrentUserAsync()
        {
            var idStr = await SecureStorage.GetAsync(CURRENT_USER_KEY);
            if (string.IsNullOrEmpty(idStr)) return null;
            if (!int.TryParse(idStr, out int userId)) return null;

            return await _userRepo.GetByIdAsync(userId);
        }

        // ✅ Wrapper sync (hạn chế dùng, nhưng không còn .Wait() nữa)
        public User? GetCurrentUser()
        {
            return GetCurrentUserAsync().GetAwaiter().GetResult();
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var idStr = await SecureStorage.GetAsync(CURRENT_USER_KEY);
            return !string.IsNullOrEmpty(idStr);
        }

        public bool IsLoggedIn()
        {
            return IsLoggedInAsync().GetAwaiter().GetResult();
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return (false, "Bạn chưa đăng nhập.");

            bool oldMatch = BCrypt.Net.BCrypt.Verify(oldPassword, currentUser.PasswordHash);
            if (!oldMatch)
                return (false, "Mật khẩu cũ không đúng.");

            currentUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var result = await _userRepo.UpdateAsync(currentUser);

            return result
                ? (true, "Đổi mật khẩu thành công.")
                : (false, "Đổi mật khẩu thất bại.");
        }
        public async Task<(bool Success, string Message)> RegisterAsync(
                                                                        string username,
                                                                        string password,
                                                                        string fullName,
                                                                        string? phone,
                                                                        string? email)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fullName))
            {
                return (false, "Vui lòng nhập đầy đủ thông tin bắt buộc.");
            }

            // kiểm tra trùng username
            var existed = await _userRepo.GetByUsernameAsync(username);
            if (existed != null)
            {
                return (false, "Tên đăng nhập đã tồn tại.");
            }

            // hash mật khẩu mới bằng BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Username = username.Trim(),
                PasswordHash = passwordHash,
                FullName = fullName.Trim(),
                Phone = phone?.Trim(),
                Email = email?.Trim(),
                Role = "Staff",          // tài khoản đăng ký mới là Nhân viên
                IsActive = true
            };

            var added = await _userRepo.AddAsync(newUser);

            return added.Id > 0
                ? (true, "Đăng ký tài khoản thành công.")
                : (false, "Đăng ký tài khoản thất bại.");
        }

    }
}
