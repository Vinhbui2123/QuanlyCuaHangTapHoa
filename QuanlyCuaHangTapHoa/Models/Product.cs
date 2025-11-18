using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Thông tin sản phẩm / hàng hóa
    /// </summary>
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty; // mã hàng / mã vạch

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        // FK -> Category
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        [StringLength(50)]
        public string? Unit { get; set; } // chai, lon, kg,...

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }

        public int StockQuantity { get; set; }

        [StringLength(255)]
        public string? ImagePath { get; set; }

        // "InStock", "LowStock", "OutOfStock" (sau này có thể làm enum + converter)
        [StringLength(20)]
        public string Status { get; set; } = "InStock";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
