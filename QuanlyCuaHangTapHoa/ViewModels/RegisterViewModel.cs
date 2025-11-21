using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanlyCuaHangTapHoa.Services;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IAuthenticationService _authService;

        [ObservableProperty] private string username = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string confirmPassword = string.Empty;
        [ObservableProperty] private string fullName = string.Empty;
        [ObservableProperty] private string? phone;
        [ObservableProperty] private string? email;
        [ObservableProperty] private string message = string.Empty;
        [ObservableProperty] private bool isBusy;

        public RegisterViewModel(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            if (Password != ConfirmPassword)
            {
                Message = "Mật khẩu xác nhận không khớp.";
                IsBusy = false;
                return;
            }

            var result = await _authService.RegisterAsync(
                Username, Password, FullName, Phone, Email);

            Message = result.Message;

            if (result.Success)
            {
                // quay lại màn hình đăng nhập
                await Shell.Current.GoToAsync("..");
            }

            IsBusy = false;
        }
    }
}
