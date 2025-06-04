using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.controllers.QuanLy
{
    [ApiController]
    [Route("api/[controller]")]
    public class QLHoaDon : ControllerBase
    {
        MyDbContext db;
        public QLHoaDon(MyDbContext context)
        {
            db = context;
        }


        [HttpGet("GetHoaDons/{idChu}")]
        public IActionResult GetHoaDons(int idChu)
        {
            var HoaDons = db.HoaDons.Join(db.Phongs, hd => hd.IdPhong, p => p.IdPhong, (hd, p) =>new { hd,p}).Join(db.CoSos, hd=>hd.p.IdCoSo,cs=>cs.IdCoSo,(hd,cs)=>new{hd,cs})
                .Join(db.Chus, hd => hd.cs.IdChu, chu => chu.IdChu, (hd, chu) => new { hd, chu })
                .Where(t => t.chu.IdChu == idChu)
                .Select(data => new
                {
                    data.hd.hd.hd.IdHoaDon,
                    data.hd.hd.hd.SoTien,
                    data.hd.hd.hd.NgayThanhToan,
                    data.hd.hd.hd.TienDien,
                    data.hd.hd.hd.TienNuoc,
                    data.hd.hd.hd.IdPhong,
                    data.hd.hd.p.TenPhong,
                    data.hd.cs.TenCoSo,
                    data.hd.cs.IdCoSo,
                    data.hd.hd.hd.TrangThai

                }).ToList();
            
            return Ok(HoaDons);
        }

    }
}