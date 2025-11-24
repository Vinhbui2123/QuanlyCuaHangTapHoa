using QuanlyCuaHangTapHoa.Models;
using QuanlyCuaHangTapHoa.ViewModels;

namespace QuanlyCuaHangTapHoa.Views;

public partial class SalesHistoryPage : ContentPage
{
    private readonly SalesHistoryViewModel _vm;

    public SalesHistoryPage(SalesHistoryViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.RefreshCommand.Execute(null);
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var sale = e.CurrentSelection.FirstOrDefault() as Sale;
        if (sale == null) return;

        _vm.OpenDetailCommand.Execute(sale);

        ((CollectionView)sender).SelectedItem = null;
    }
}
