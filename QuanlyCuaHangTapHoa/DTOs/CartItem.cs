using QuanlyCuaHangTapHoa.Models;

namespace QuanlyCuaHangTapHoa.DTOs
{
    public class CartItem
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }

        public decimal TotalPrice => UnitPrice * Quantity - DiscountAmount;
    }
}
