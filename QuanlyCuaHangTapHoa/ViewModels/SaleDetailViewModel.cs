using CommunityToolkit.Mvvm.ComponentModel;
using QuanlyCuaHangTapHoa.Data;
using QuanlyCuaHangTapHoa.DTOs;
using QuanlyCuaHangTapHoa.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.ViewModels
{
    public partial class SaleDetailViewModel : ObservableObject
    {
        private readonly AppDbContext _db;

        [ObservableProperty] private string saleCode = string.Empty;
        [ObservableProperty] private DateTime saleDate;
        [ObservableProperty] private decimal finalAmount;
        [ObservableProperty] private decimal receivedAmount;
        [ObservableProperty] private decimal changeAmount;
        [ObservableProperty] private string paymentMethod = string.Empty;
        [ObservableProperty] private string? notes;
        [ObservableProperty] private ObservableCollection<SaleDetailItemDto> items = new();
        [ObservableProperty] private string message = string.Empty;
        [ObservableProperty] private bool isBusy;

        public SaleDetailViewModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task LoadAsync(int saleId)
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            try
            {
                var sale = await _db.Sales.FirstOrDefaultAsync(s => s.Id == saleId);
                if (sale == null)
                {
                    Message = "Không tìm thấy hóa đơn.";
                    IsBusy = false;
                    return;
                }

                SaleCode = sale.Code;
                SaleDate = sale.Date;
                FinalAmount = sale.FinalAmount;
                ReceivedAmount = sale.ReceivedAmount;
                ChangeAmount = sale.ChangeAmount;
                PaymentMethod = sale.PaymentMethod;
                Notes = sale.Notes;

                var details = await _db.SaleDetails
                    .Include(d => d.Product)
                    .Where(d => d.SaleId == saleId)
                    .ToListAsync();

                var list = details.ConvertAll(d => new SaleDetailItemDto
                {
                    ProductName = d.Product?.Name ?? $"SP#{d.ProductId}",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    DiscountAmount = d.DiscountAmount,
                    TotalPrice = d.TotalPrice
                });

                Items = new ObservableCollection<SaleDetailItemDto>(list);
            }
            catch (Exception ex)
            {
                Message = "Không tải được chi tiết hóa đơn.";
                Debug.WriteLine("[SaleDetailVM] " + ex.Message);
            }

            IsBusy = false;
        }
    }
}
