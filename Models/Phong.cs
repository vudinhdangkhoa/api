using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Phong
{
    public int IdPhong { get; set; }

    public string? TenPhong { get; set; }

    public int SoLuong { get; set; }

    public int IdCoSo { get; set; }

    public int? TrangThai { get; set; }

    public double? TienPhong { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual CoSo IdCoSoNavigation { get; set; } = null!;

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
}
