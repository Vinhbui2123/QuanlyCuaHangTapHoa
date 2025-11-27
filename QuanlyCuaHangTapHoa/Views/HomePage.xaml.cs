using QuanlyCuaHangTapHoa.Services;
using QuanlyCuaHangTapHoa.ViewModels;

namespace QuanlyCuaHangTapHoa.Views;

public partial class HomePage : ContentPage
{
    private readonly IAuthenticationService _authService;
    private readonly HomePageViewModel _vm;

    public HomePage(IAuthenticationService authService, HomePageViewModel vm)
    {
        InitializeComponent();

        _authService = authService;
        _vm = vm;

        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadDashboardAsync();
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        // Xóa thông tin đăng nhập
        await _authService.LogoutAsync();

        // Khóa menu chính
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;

        // Điều hướng về login + reset navigation stack
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
