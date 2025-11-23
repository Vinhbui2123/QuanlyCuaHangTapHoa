using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanlyCuaHangTapHoa.Models;
using QuanlyCuaHangTapHoa.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using QuanlyCuaHangTapHoa.Views;
using Microsoft.Maui.Controls;
using System.Linq;

namespace QuanlyCuaHangTapHoa.ViewModels
{
    public partial class ProductListViewModel : ObservableObject
    {
        private readonly IProductService _productService;

        [ObservableProperty]
        private ObservableCollection<Product> products = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string message = string.Empty;

        public ProductListViewModel(IProductService productService)
        {
            _productService = productService;

            // Load dữ liệu lần đầu
            LoadProductsCommand.Execute(null);
        }

        [RelayCommand]
        public async Task LoadProducts()
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            var list = await _productService.GetAllAsync();
            Products = new ObservableCollection<Product>(list);

            IsBusy = false;
        }

        [RelayCommand]
        public async Task Search()
        {
            if (IsBusy) return;
            IsBusy = true;

            var keyword = SearchText?.Trim() ?? string.Empty;

            var list = string.IsNullOrWhiteSpace(keyword)
                ? await _productService.GetAllAsync()
                : await _productService.SearchAsync(keyword);

            Products = new ObservableCollection<Product>(list);

            IsBusy = false;
        }

        [RelayCommand]
        public async Task Refresh()
        {
            await LoadProducts();
        }

        [RelayCommand]
        public async Task GoToAddProduct()
        {
            await Shell.Current.GoToAsync(nameof(Views.ProductDetailPage));
        }
        [RelayCommand]
        public async Task GoToEditProduct(Product product)
        {
            if (product == null) return;

            await Shell.Current.GoToAsync(
                $"{nameof(ProductDetailPage)}?productId={product.Id}");
        }

    }
}
