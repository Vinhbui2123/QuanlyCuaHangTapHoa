using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanlyCuaHangTapHoa.Data;
using QuanlyCuaHangTapHoa.Models;
using QuanlyCuaHangTapHoa.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace QuanlyCuaHangTapHoa.ViewModels
{
    public partial class ProductDetailViewModel : ObservableObject
    {
        private readonly IProductService _productService;
        private readonly AppDbContext _db;

        // Id sản phẩm (0 = thêm mới, >0 = chỉnh sửa)
        [ObservableProperty]
        private int id;

        [ObservableProperty] private string code = string.Empty;
        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private string? unit;
        [ObservableProperty] private decimal purchasePrice;
        [ObservableProperty] private decimal sellingPrice;
        [ObservableProperty] private int stockQuantity;
        [ObservableProperty] private string status = "InStock";

        [ObservableProperty] private ObservableCollection<Category> categories = new();
        [ObservableProperty] private Category? selectedCategory;

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string message = string.Empty;

        public ProductDetailViewModel(IProductService productService, AppDbContext db)
        {
            _productService = productService;
            _db = db;

            _ = LoadCategoriesAsync();
        }

        // --------- LOAD CATEGORY ---------
        private async Task LoadCategoriesAsync()
        {
            try
            {
                var list = await _db.Categories.ToListAsync();
                Categories = new ObservableCollection<Category>(list);
                if (Categories.Count > 0 && SelectedCategory == null)
                    SelectedCategory = Categories[0];
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[ProductDetailVM] LoadCategories error: {ex.Message}");
                Message = "Không tải được danh sách nhóm hàng.";
            }
        }

        // --------- LOAD DỮ LIỆU KHI SỬA ---------
        public async Task LoadProductAsync(int productId)
        {
            try
            {
                if (productId <= 0) return;

                var product = await _productService.GetByIdAsync(productId);
                if (product == null) return;

                Id = product.Id;
                Code = product.Code;
                Name = product.Name;
                Unit = product.Unit;
                PurchasePrice = product.PurchasePrice;
                SellingPrice = product.SellingPrice;
                StockQuantity = product.StockQuantity;
                Status = product.Status;

                if (Categories.Count == 0)
                    await LoadCategoriesAsync();

                SelectedCategory = Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[ProductDetailVM] LoadProductAsync error: {ex.Message}");
                Message = "Không tải được thông tin sản phẩm.";
            }
        }

        // --------- LƯU (THÊM / SỬA) ---------
        [RelayCommand]
        public async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Code))
            {
                Message = "Mã và tên sản phẩm không được để trống.";
                IsBusy = false;
                return;
            }

            if (SelectedCategory == null)
            {
                Message = "Vui lòng chọn nhóm hàng.";
                IsBusy = false;
                return;
            }

            var product = new Product
            {
                Id = Id, // 0 = thêm, >0 = sửa
                Code = Code.Trim(),
                Name = Name.Trim(),
                CategoryId = SelectedCategory.Id,
                Unit = Unit?.Trim(),
                PurchasePrice = PurchasePrice,
                SellingPrice = SellingPrice,
                StockQuantity = StockQuantity,
                Status = Status,
                IsActive = true
            };

            var result = await _productService.SaveAsync(product);
            Message = result.Message;

            if (result.Success)
            {
                await Shell.Current.GoToAsync("..");
            }

            IsBusy = false;
        }

        // --------- XÓA ---------
        [RelayCommand]
        public async Task DeleteAsync()
        {
            if (Id == 0)
            {
                Message = "Sản phẩm chưa được lưu, không thể xóa.";
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Xóa sản phẩm",
                "Bạn có chắc chắn muốn xóa sản phẩm này?",
                "Có", "Không");

            if (!confirm) return;

            var result = await _productService.DeleteAsync(Id);
            Message = result.Message;

            if (result.Success)
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
