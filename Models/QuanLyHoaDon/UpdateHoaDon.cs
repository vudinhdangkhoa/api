using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.QuanLyHoaDon
{
    public class UpdateHoaDon
    {
        public int maHoaDon { get; set; }
        public int soDienCu { get; set; }
        public int soDienMoi { get; set; }

        public int idChu { get; set; }
    }
}