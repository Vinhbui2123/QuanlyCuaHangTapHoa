using System.ComponentModel.DataAnnotations.Schema;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Chi tiết từng dòng hàng trong phiếu nhập
    /// </summary>
    public class PurchaseOrderDetail
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }
        public virtual PurchaseOrder? PurchaseOrder { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
    }
}
