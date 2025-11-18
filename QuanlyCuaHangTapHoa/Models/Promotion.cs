using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Chương trình khuyến mãi
    /// </summary>
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        /// <summary>
        /// 0: Theo sản phẩm, 1: Theo hóa đơn
        /// </summary>
        public int Type { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPercent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinInvoiceAmount { get; set; }

        public int? ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
