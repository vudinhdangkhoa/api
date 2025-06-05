using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var HoaDons = db.HoaDons.Join(db.Phongs, hd => hd.IdPhong, p => p.IdPhong, (hd, p) => new { hd, p }).Join(db.CoSos, hd => hd.p.IdCoSo, cs => cs.IdCoSo, (hd, cs) => new { hd, cs })
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

        [HttpGet("AddHoaDon/{idChu}")]
        public async Task<IActionResult> AddHoaDon(int idChu)
        {
            string a = DateOnly.FromDateTime(DateTime.Now).ToString("MM");
            var HoaDons = db.HoaDons.Join(db.Phongs, hd => hd.IdPhong, p => p.IdPhong, (hd, p) => new { hd, p }).Join(db.CoSos, hd => hd.p.IdCoSo, cs => cs.IdCoSo, (hd, cs) => new { hd, cs })
                
                .Where(t => t.cs.IdChu == idChu && t.hd.hd.NgayTao.HasValue && t.hd.hd.NgayTao.Value.Month == DateTime.Now.Month 
        && t.hd.hd.NgayTao.Value.Year == DateTime.Now.Year ).Select(
                    data => new
                    {
                        data.hd.hd.NgayTao
                    }
                )
                .ToList();
            if (HoaDons.Count > 0)
            {
                return BadRequest(HoaDons);
            }
            else
            {
                await db.Database.ExecuteSqlRawAsync("EXEC TaoHoaDonTuDong @idChu = {0}", idChu);
                return Ok(new { message = "đã tạo hóa đơn cho tháng " + DateTime.Now.Month + " rồi" });
            }

            return Ok();
        }


    }
}