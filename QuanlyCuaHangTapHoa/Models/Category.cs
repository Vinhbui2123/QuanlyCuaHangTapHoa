using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Nhóm / loại sản phẩm (đồ uống, gia vị, nhu yếu phẩm,...)
    /// </summary>
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation: một Category có nhiều Product
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
