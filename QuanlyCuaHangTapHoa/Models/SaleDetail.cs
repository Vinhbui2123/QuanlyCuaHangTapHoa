using System.ComponentModel.DataAnnotations.Schema;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Chi tiết từng sản phẩm trong hóa đơn bán hàng
    /// </summary>
    public class SaleDetail
    {
        public int Id { get; set; }

        public int SaleId { get; set; }
        public virtual Sale? Sale { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
    }
}
