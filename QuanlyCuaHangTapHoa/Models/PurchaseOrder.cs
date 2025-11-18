using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Phiếu nhập hàng từ nhà cung cấp
    /// </summary>
    public class PurchaseOrder
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty; // PN0001...

        public int SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        public int CreatedByUserId { get; set; }
        public virtual User? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DebtAmount { get; set; }

        /// <summary>
        /// 0: Draft, 1: Completed, 2: Cancelled
        /// </summary>
        public int Status { get; set; } = 1;

        [StringLength(255)]
        public string? Note { get; set; }

        public virtual ICollection<PurchaseOrderDetail> Details { get; set; } = new List<PurchaseOrderDetail>();
    }
}
