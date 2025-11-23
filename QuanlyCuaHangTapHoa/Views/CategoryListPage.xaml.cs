using QuanlyCuaHangTapHoa.Models;
using QuanlyCuaHangTapHoa.ViewModels;

namespace QuanlyCuaHangTapHoa.Views;

public partial class CategoryListPage : ContentPage
{
    private readonly CategoryListViewModel _vm;

    public CategoryListPage(CategoryListViewModel vm)
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
        var category = e.CurrentSelection.FirstOrDefault() as Category;
        if (category == null) return;

        _vm.GoToEditCommand.Execute(category);

        ((CollectionView)sender).SelectedItem = null;
    }
}
    