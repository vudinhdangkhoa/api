using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.controllers.ThongKe
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongKeDoanhThu : ControllerBase
    {
        MyDbContext db;
        public ThongKeDoanhThu(MyDbContext context)
        {
            db = context;
        }



        [HttpGet("GetDoanhThu/{idChu}")]
        public async Task<IActionResult> GetDoanhThu(int idChu)
        {
            try
            {
                var doanhthu = db.HoaDons.Join(db.Phongs, hd => hd.IdPhong, p => p.IdPhong, (hd, p) => new { hd, p })
                .Join(db.CoSos, hd_p => hd_p.p.IdCoSo, cs => cs.IdCoSo, (hd_p, cs) => new { hd_p, cs })
                .Join(db.Chus, hd_p_cs => hd_p_cs.cs.IdChu, chu => chu.IdChu, (hd_p_cs, chu) => new { hd_p_cs, chu })
                .Where(t => t.chu.IdChu == idChu &&
                 t.hd_p_cs.hd_p.hd.NgayThanhToan.HasValue &&
                 t.hd_p_cs.hd_p.hd.NgayThanhToan.Value.Year == DateTime.Now.Year &&
                 t.hd_p_cs.hd_p.hd.TrangThai == 1).GroupBy(
                    data => new
                    {

                        thang = data.hd_p_cs.hd_p.hd.NgayThanhToan.Value.Month,
                        nam = data.hd_p_cs.hd_p.hd.NgayThanhToan.Value.Year,
                    }
                 )
                 .Select(data => new
                 {


                     thang = data.Key.thang,
                     nam = data.Key.nam,
                     tongtien = data.Sum(t =>
                     t.hd_p_cs.hd_p.hd.SoTien +
                     t.hd_p_cs.hd_p.hd.TienNuoc +
                     t.hd_p_cs.hd_p.hd.TienDien * t.chu.GiaDien)

                 })
                 .OrderBy(g => g.nam).ThenBy(g => g.thang)
                .ToList();
                return Ok(doanhthu);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

    }
}