using QuanlyCuaHangTapHoa.Views;

namespace QuanlyCuaHangTapHoa;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // đăng ký route cho RegisterPage
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

        // đăng ký route cho ProductDetailPage
        Routing.RegisterRoute(nameof(ProductDetailPage), typeof(ProductDetailPage));
    }
}
