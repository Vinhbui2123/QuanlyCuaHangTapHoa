using QuanlyCuaHangTapHoa.ViewModels;

namespace QuanlyCuaHangTapHoa.Views;
using QuanlyCuaHangTapHoa.Models;
using System.Linq;

public partial class ProductListPage : ContentPage
{
    private readonly ProductListViewModel _vm;

    public ProductListPage(ProductListViewModel vm)
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

    private void SearchEntry_Completed(object sender, EventArgs e)
    {
        _vm.SearchCommand.Execute(null);
    }
    //private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    var product = e.CurrentSelection.FirstOrDefault() as Product;
    //    if (product == null) return;

    //    _vm.GoToEditProductCommand.Execute(product);

    //    ((CollectionView)sender).SelectedItem = null; // bỏ chọn để lần sau tap vẫn ăn
    //}

}
