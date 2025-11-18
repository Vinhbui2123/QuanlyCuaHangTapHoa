using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Hóa đơn bán hàng (POS)
    /// </summary>
    public class Sale
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty; // HD0001...

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

        public int UserId { get; set; }
        public virtual User? User { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal ChangeAmount { get; set; }

        /// <summary>
        /// "Cash" hoặc "BankTransfer"
        /// </summary>
        [StringLength(30)]
        public string PaymentMethod { get; set; } = "Cash";

        [StringLength(255)]
        public string? Notes { get; set; }

        public virtual ICollection<SaleDetail> Details { get; set; } = new List<SaleDetail>();
    }
}
