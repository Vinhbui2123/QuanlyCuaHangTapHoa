using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanlyCuaHangTapHoa.DTOs;
using QuanlyCuaHangTapHoa.Models;
using QuanlyCuaHangTapHoa.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuanlyCuaHangTapHoa.ViewModels
{
    public partial class PosViewModel : ObservableObject
    {
        private readonly IProductService _productService;
        private readonly ISalesService _salesService;
        private readonly IAuthenticationService _authService;

        [ObservableProperty] private ObservableCollection<CartItem> cartItems = new();
        [ObservableProperty] private string searchCodeOrName = string.Empty;
        [ObservableProperty] private decimal totalAmount;
        [ObservableProperty] private decimal totalDiscount;
        [ObservableProperty] private decimal finalAmount;
        [ObservableProperty] private decimal receivedAmount;
        [ObservableProperty] private decimal changeAmount;
        [ObservableProperty] private string message = string.Empty;
        [ObservableProperty] private bool isBusy;

        public PosViewModel(
            IProductService productService,
            ISalesService salesService,
            IAuthenticationService authService)
        {
            _productService = productService;
            _salesService = salesService;
            _authService = authService;
        }

        private void RecalculateTotals()
        {
            TotalAmount = CartItems.Sum(i => i.UnitPrice * i.Quantity);
            TotalDiscount = CartItems.Sum(i => i.DiscountAmount);
            FinalAmount = TotalAmount - TotalDiscount;
            if (FinalAmount < 0) FinalAmount = 0;

            ChangeAmount = ReceivedAmount - FinalAmount;
            if (ChangeAmount < 0) ChangeAmount = 0;
        }

        partial void OnReceivedAmountChanged(decimal value)
        {
            RecalculateTotals();
        }

        // ===== Thêm sản phẩm vào giỏ theo mã hoặc tên =====
        [RelayCommand]
        public async Task AddProductAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            var keyword = SearchCodeOrName?.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                Message = "Vui lòng nhập mã hoặc tên sản phẩm.";
                IsBusy = false;
                return;
            }

            try
            {
                var listByName = await _productService.SearchAsync(keyword);
                var p = listByName.FirstOrDefault();

                if (p == null)
                {
                    Message = "Không tìm thấy sản phẩm.";
                    IsBusy = false;
                    return;
                }

                // Nếu product đang có trong giỏ
                var existing = CartItems.FirstOrDefault(ci => ci.Product.Id == p.Id);

                if (existing != null)
                {
                    // 🔥 Không cho vượt tồn kho
                    if (existing.Quantity >= p.StockQuantity)
                    {
                        Message = $"Sản phẩm '{p.Name}' chỉ còn {p.StockQuantity} {p.Unit} trong kho.";
                        IsBusy = false;
                        return;
                    }

                    existing.Quantity += 1;
                    OnPropertyChanged(nameof(CartItems));
                }
                else
                {
                    // 🔥 Nếu hết hàng → không cho thêm
                    if (p.StockQuantity <= 0)
                    {
                        Message = $"Sản phẩm '{p.Name}' đã hết hàng.";
                        IsBusy = false;
                        return;
                    }

                    CartItems.Add(new CartItem
                    {
                        Product = p,
                        Quantity = 1,
                        UnitPrice = p.SellingPrice,
                        DiscountAmount = 0
                    });
                }

                SearchCodeOrName = string.Empty;
                RecalculateTotals();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            IsBusy = false;
        }


        [RelayCommand]
        public void IncreaseQuantity(CartItem item)
        {
            if (item == null) return;

            // Không cho vượt tồn kho hiện tại
            if (item.Quantity >= item.Product.StockQuantity)
            {
                Message = $"Sản phẩm '{item.Product.Name}' chỉ còn {item.Product.StockQuantity} {item.Product.Unit} trong kho.";
                return;
            }

            item.Quantity += 1;
            OnPropertyChanged(nameof(CartItems));
            RecalculateTotals();
        }


        [RelayCommand]
        public void DecreaseQuantity(CartItem item)
        {
            if (item == null) return;
            if (item.Quantity > 1)
                item.Quantity -= 1;
            else
                CartItems.Remove(item);

            OnPropertyChanged(nameof(CartItems));
            RecalculateTotals();
        }

        [RelayCommand]
        public void RemoveItem(CartItem item)
        {
            if (item == null) return;
            CartItems.Remove(item);
            RecalculateTotals();
        }

        // ===== Thanh toán =====
        [RelayCommand]
        public async Task CheckoutAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            if (CartItems.Count == 0)
            {
                Message = "Giỏ hàng đang trống.";
                IsBusy = false;
                return;
            }

            var currentUser = await _authService.GetCurrentUserAsync();

            if (currentUser == null)
            {
                Message = "Phiên đăng nhập đã hết, vui lòng đăng nhập lại.";
                IsBusy = false;
                return;
            }

            if (ReceivedAmount < FinalAmount)
            {
                Message = "Số tiền khách đưa chưa đủ.";
                IsBusy = false;
                return;
            }

            try
            {
                var items = CartItems.Select(ci =>
                    (ci.Product.Id, ci.Quantity, ci.UnitPrice, ci.DiscountAmount)).ToList();

                var sale = await _salesService.CreateSaleAsync(
                    items,
                    customerId: null,
                    userId: currentUser.Id,
                    receivedAmount: ReceivedAmount,
                    paymentMethod: "Cash",
                    notes: null);

                Message = $"Thanh toán thành công. Mã hóa đơn: {sale.Code}";

                CartItems.Clear();
                ReceivedAmount = 0;
                RecalculateTotals();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"[PosViewModel] CheckoutAsync error: {ex.Message}");
                Message = ex.Message;
            }

            IsBusy = false;
        }
    }
}
