using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using QuanlyCuaHangTapHoa.Models;
using QuanlyCuaHangTapHoa.Services;
using System.Threading.Tasks;


namespace QuanlyCuaHangTapHoa.ViewModels
{
    public partial class CategoryDetailViewModel : ObservableObject
    {
        private readonly ICategoryService _categoryService;

        [ObservableProperty] private int id;
        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private bool isActive = true;
        [ObservableProperty] private string message = string.Empty;
        [ObservableProperty] private bool isBusy;

        public CategoryDetailViewModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task LoadAsync(int categoryId)
        {
            if (categoryId <= 0) return;

            var c = await _categoryService.GetByIdAsync(categoryId);
            if (c == null) return;

            Id = c.Id;
            Name = c.Name;
            IsActive = c.IsActive;
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            var category = new Category
            {
                Id = Id,
                Name = Name,
                IsActive = IsActive
            };

            var result = await _categoryService.SaveAsync(category);
            Message = result.Message;

            if (result.Success)
            {
                await Shell.Current.GoToAsync("..");
            }

            IsBusy = false;
        }

        [RelayCommand]
        public async Task DeleteAsync()
        {
            if (Id == 0)
            {
                Message = "Danh mục chưa được lưu, không thể xóa.";
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Xóa danh mục",
                "Bạn có chắc chắn muốn xóa danh mục này?",
                "Có", "Không");

            if (!confirm) return;

            var result = await _categoryService.DeleteAsync(Id);
            Message = result.Message;

            if (result.Success)
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
