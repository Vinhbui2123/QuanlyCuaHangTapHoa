using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanlyCuaHangTapHoa.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.ViewModels
{
    public partial class HomePageViewModel : ObservableObject
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISalesService _salesService;

        [ObservableProperty]
        private int totalProducts;

        [ObservableProperty]
        private int totalCategories;

        [ObservableProperty]
        private decimal todayRevenue;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string message;

        public HomePageViewModel(
            IProductService productService,
            ICategoryService categoryService,
            ISalesService salesService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _salesService = salesService;
        }

        /// <summary>
        /// Load số liệu dashboard (gọi trong OnAppearing của HomePage)
        /// </summary>
        public async Task LoadDashboardAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            try
            {
                // Tổng sản phẩm
                var products = await _productService.GetAllAsync();
                TotalProducts = products?.Count() ?? 0;

                // Tổng danh mục
                var categories = await _categoryService.GetAllAsync();
                TotalCategories = categories?.Count() ?? 0;

                // Doanh thu hôm nay
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                var sales = await _salesService.GetSalesByDateRangeAsync(today, tomorrow);
                TodayRevenue = sales?.Sum(s => s.FinalAmount) ?? 0;


                TodayRevenue = sales?.Sum(s => s.FinalAmount) ?? 0;
            }
            catch (Exception ex)
            {
                Message = $"Lỗi tải dashboard: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        // 🚀 Menu nhanh: điều hướng tới các màn chính

        [RelayCommand]
        private async Task GoProducts()
        {
            await Shell.Current.GoToAsync(nameof(Views.ProductListPage));
        }

        [RelayCommand]
        private async Task GoPos()
        {
            await Shell.Current.GoToAsync(nameof(Views.PosPage));
        }

        [RelayCommand]
        private async Task GoCategories()
        {
            await Shell.Current.GoToAsync(nameof(Views.CategoryListPage));
        }

        [RelayCommand]
        private async Task GoHistory()
        {
            await Shell.Current.GoToAsync(nameof(Views.SalesHistoryPage));
        }
    }
}
