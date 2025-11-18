using System;
using System.ComponentModel.DataAnnotations;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Lịch sử biến động tồn kho
    /// </summary>
    public class StockMovement
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 0: IN, 1: OUT, 2: ADJUST
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Số lượng thay đổi (dương: nhập, âm: xuất)
        /// </summary>
        public int QuantityChange { get; set; }

        [StringLength(50)]
        public string? ReferenceType { get; set; } // "Sale" / "Purchase" / "Adjustment"

        public int? ReferenceId { get; set; }

        /// <summary>
        /// Tồn kho sau khi thực hiện giao dịch
        /// </summary>
        public int CurrentStockAfter { get; set; }

        [StringLength(255)]
        public string? Note { get; set; }
    }
}
