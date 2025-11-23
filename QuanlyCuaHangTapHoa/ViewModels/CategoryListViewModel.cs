using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanlyCuaHangTapHoa.Models;
using QuanlyCuaHangTapHoa.Services;
using QuanlyCuaHangTapHoa.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.ViewModels
{
    public partial class CategoryListViewModel : ObservableObject
    {
        private readonly ICategoryService _categoryService;

        [ObservableProperty] private ObservableCollection<Category> categories = new();
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string message = string.Empty;

        public CategoryListViewModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
            _ = LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            var list = await _categoryService.GetAllAsync();
            Categories = new ObservableCollection<Category>(list);

            IsBusy = false;
        }

        [RelayCommand]
        public async Task RefreshAsync()
        {
            await LoadAsync();
        }

        [RelayCommand]
        public async Task GoToAddAsync()
        {
            await Shell.Current.GoToAsync(nameof(CategoryDetailPage));
        }

        [RelayCommand]
        public async Task GoToEditAsync(Category category)
        {
            if (category == null) return;

            await Shell.Current.GoToAsync(
                $"{nameof(CategoryDetailPage)}?categoryId={category.Id}");
        }
    }
}
