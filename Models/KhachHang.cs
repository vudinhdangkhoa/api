using System;
using System.Collections.Generic;

namespace api.Models;

public partial class KhachHang
{
    public int IdKh { get; set; }

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

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual Phong IdPhongNavigation { get; set; } = null!;
}
