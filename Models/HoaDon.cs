using System;
using System.Collections.Generic;

namespace api.Models;

public partial class HoaDon
{
    public int IdHoaDon { get; set; }

    public double? SoTien { get; set; }

    public int IdPhong { get; set; }

    public int? TrangThai { get; set; }

    public DateOnly? NgayThanhToan { get; set; }

    public double? TienDien { get; set; }

    public double? TienNuoc { get; set; }

    public DateOnly? NgayTao { get; set; }

    public virtual Phong IdPhongNavigation { get; set; } = null!;
}
