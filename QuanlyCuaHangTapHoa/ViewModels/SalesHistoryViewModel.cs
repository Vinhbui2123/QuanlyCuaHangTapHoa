using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanlyCuaHangTapHoa.Models;
using QuanlyCuaHangTapHoa.Services;
using QuanlyCuaHangTapHoa.Views;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.ViewModels
{
    public partial class SalesHistoryViewModel : ObservableObject
    {
        private readonly ISalesService _salesService;

        [ObservableProperty] private ObservableCollection<Sale> sales = new();
        [ObservableProperty] private DateTime fromDate = DateTime.Today.AddDays(-7);
        [ObservableProperty] private DateTime toDate = DateTime.Today;
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string message = string.Empty;
        [ObservableProperty] private decimal totalRevenue;

        public SalesHistoryViewModel(ISalesService salesService)
        {
            _salesService = salesService;
            _ = LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            try
            {
                var list = await _salesService.GetSalesByDateRangeAsync(FromDate, ToDate);
                Sales = new ObservableCollection<Sale>(list);

                TotalRevenue = list.Sum(s => s.FinalAmount);
                if (!Sales.Any())
                    Message = "Không có hóa đơn nào trong khoảng thời gian này.";
            }
            catch (Exception ex)
            {
                Message = "Không tải được danh sách hóa đơn.";
                System.Diagnostics.Debug.WriteLine("[SalesHistoryVM] " + ex.Message);
            }

            IsBusy = false;
        }

        [RelayCommand]
        public async Task RefreshAsync()
        {
            await LoadAsync();
        }

        [RelayCommand]
        public async Task OpenDetailAsync(Sale sale)
        {
            if (sale == null) return;

            await Shell.Current.GoToAsync(
                $"{nameof(SaleDetailPage)}?saleId={sale.Id}");
        }
    }
}
