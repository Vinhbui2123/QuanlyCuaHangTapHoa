using QuanlyCuaHangTapHoa.Services;

namespace QuanlyCuaHangTapHoa.Views;

public partial class HomePage : ContentPage
{
    private readonly IAuthenticationService _authService;

    public HomePage(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        // Xóa thông tin đăng nhập
        await _authService.LogoutAsync();

        // Khóa lại menu
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;

        // Điều hướng về màn đăng nhập, reset stack
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
