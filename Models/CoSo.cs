using System;
using System.Collections.Generic;

namespace api.Models;

public partial class CoSo
{
    public int IdCoSo { get; set; }

    public string TenCoSo { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public int IdChu { get; set; }

    public int? TrangThai { get; set; }

    public int? SoLuong { get; set; }

    public virtual Chu IdChuNavigation { get; set; } = null!;

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
