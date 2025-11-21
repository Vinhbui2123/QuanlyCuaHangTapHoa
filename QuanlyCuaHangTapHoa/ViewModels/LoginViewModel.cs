using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanlyCuaHangTapHoa.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using QuanlyCuaHangTapHoa.Views;
using Microsoft.Maui.Controls;

namespace QuanlyCuaHangTapHoa.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthenticationService _authService;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string message = string.Empty;

        public LoginViewModel(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        public async Task LoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            var result = await _authService.LoginAsync(Username, Password);

            if (!result.Success)
            {
                Message = result.Message;
                IsBusy = false;
                return;
            }

            // Điều hướng sang trang chính sau khi đăng nhập
            await Shell.Current.GoToAsync("//HomePage");

            IsBusy = false;
        }
        [RelayCommand]
        public async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
        }
    }
}
