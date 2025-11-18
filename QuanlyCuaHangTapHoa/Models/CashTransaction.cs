using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Ghi nhận thu - chi tiền mặt
    /// </summary>
    public class CashTransaction
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 0: Thu, 1: Chi
        /// </summary>
        public int Type { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public int? RelatedSaleId { get; set; }
        public virtual Sale? RelatedSale { get; set; }

        public int? RelatedPurchaseOrderId { get; set; }
        public virtual PurchaseOrder? RelatedPurchaseOrder { get; set; }
    }
}
