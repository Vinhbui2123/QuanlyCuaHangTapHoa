using QuanlyCuaHangTapHoa.ViewModels;

namespace QuanlyCuaHangTapHoa.Views;

[QueryProperty(nameof(CategoryId), "categoryId")]
public partial class CategoryDetailPage : ContentPage
{
    private readonly CategoryDetailViewModel _vm;

    public int CategoryId { get; set; }

    public CategoryDetailPage(CategoryDetailViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (CategoryId > 0)
            await _vm.LoadAsync(CategoryId);
        else
        {
            _vm.Id = 0;
            _vm.Name = string.Empty;
            _vm.IsActive = true;
            _vm.Message = string.Empty;
        }
    }
}
