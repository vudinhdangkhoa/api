using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DangNhapDangKy
{
    public class DangKyKH
    {
        public string? TenKh { get; set; }

        public string? Avatar { get; set; }
    
        public string Sdt { get; set; } = null!;

        public string Cccd { get; set; } = null!;

        public DateOnly? NgayDen { get; set; }

        public DateOnly? NgayDi { get; set; }

        public int? Tinhtrang { get; set; }

        public string? Email { get; set; }

        public string MatKhau { get; set; } = null!;

        public int IdPhong { get; set; }
    }
}