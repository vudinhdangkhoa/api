using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Chu
{
    public int IdChu { get; set; }

    public string Ten { get; set; } = null!;

    public string TaiKhoan { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? Avatar { get; set; }

    public double? GiaNuoc { get; set; }

    public double? GiaDien { get; set; }

    public virtual ICollection<CoSo> CoSos { get; set; } = new List<CoSo>();
}
