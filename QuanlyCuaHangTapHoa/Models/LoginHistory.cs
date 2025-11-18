using System;
using System.ComponentModel.DataAnnotations;

namespace QuanlyCuaHangTapHoa.Models
{
    /// <summary>
    /// Lịch sử đăng nhập hệ thống
    /// </summary>
    public class LoginHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User? User { get; set; }

        public DateTime LoginTime { get; set; } = DateTime.UtcNow;

        public bool IsSuccess { get; set; }

        [StringLength(255)]
        public string? DeviceInfo { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }
    }
}
