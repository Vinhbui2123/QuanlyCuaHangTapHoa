using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Nhà cung cấp hàng hóa
    /// </summary>
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Note { get; set; }

        /// <summary>
        /// Nợ hiện tại với nhà cung cấp
        /// </summary>
        public decimal CurrentDebt { get; set; }

        public bool IsActive { get; set; } = true;

        // 1 Supplier - N PurchaseOrders
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
