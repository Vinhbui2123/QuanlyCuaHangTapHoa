using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Khách hàng thân thiết
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        public double LoyaltyPoints { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
