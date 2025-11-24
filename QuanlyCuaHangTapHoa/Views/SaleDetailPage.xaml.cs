using QuanlyCuaHangTapHoa.ViewModels;

namespace QuanlyCuaHangTapHoa.Views;

[QueryProperty(nameof(SaleId), "saleId")]
public partial class SaleDetailPage : ContentPage
{
    private readonly SaleDetailViewModel _vm;

    public int SaleId { get; set; }

    public SaleDetailPage(SaleDetailViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (SaleId > 0)
        {
            await _vm.LoadAsync(SaleId);
        }
    }
}
