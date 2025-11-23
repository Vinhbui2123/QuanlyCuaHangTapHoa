using QuanlyCuaHangTapHoa.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace QuanlyCuaHangTapHoa.Views;

[QueryProperty(nameof(ProductId), "productId")]
public partial class ProductDetailPage : ContentPage
{
    private readonly ProductDetailViewModel _vm;

    public int ProductId { get; set; }  // nhận từ Shell query

    public ProductDetailPage(ProductDetailViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Nếu ProductId > 0 => chế độ sửa → load dữ liệu
        if (ProductId > 0)
        {
            await _vm.LoadProductAsync(ProductId);
            Title = "Sửa sản phẩm";
        }
        else
        {
            Title = "Thêm sản phẩm";
        }
    }
}
