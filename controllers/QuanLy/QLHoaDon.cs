using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models.QuanLyHoaDon;
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
                    data.hd.hd.hd.TrangThai,
                    data.chu.GiaDien,
                    data.chu.GiaNuoc,
                    data.hd.hd.hd.AnhHoaDon

                }).ToList();

            return Ok(HoaDons);
        }

        [HttpGet("AddHoaDon/{idChu}")]
        public async Task<IActionResult> AddHoaDon(int idChu)
        {
            string a = DateOnly.FromDateTime(DateTime.Now).ToString("MM");
            var HoaDons = db.HoaDons.Join(db.Phongs, hd => hd.IdPhong, p => p.IdPhong, (hd, p) => new { hd, p })
            .Join(db.CoSos, hd => hd.p.IdCoSo, cs => cs.IdCoSo, (hd, cs) => new { hd, cs })
            .Where(t => t.cs.IdChu == idChu && t.hd.hd.NgayTao.HasValue && t.hd.hd.NgayTao.Value.Month == DateTime.Now.Month
                && t.hd.hd.NgayTao.Value.Year == DateTime.Now.Year).Select(
                    data => new
                    {
                        data.hd.hd.NgayTao
                    }
                )
                .ToList();
            if (HoaDons.Count > 0)
            {
                return BadRequest(new { message = "đã tạo hóa đơn cho tháng " + DateTime.Now.Month + " rồi" });
            }
            else
            {
                await db.Database.ExecuteSqlRawAsync("EXEC TaoHoaDonTuDong @idChu = {0}", idChu);
                return Ok(new { message = "đã tạo hóa đơn cho tháng " + DateTime.Now.Month + " rồi" });
            }


        }

        [HttpPut("UpdateHoaDon")]
        public IActionResult UpdateHoaDon([FromBody] UpdateHoaDon updateHoaDon)
        {
            Chu chu = db.Chus.FirstOrDefault(t => t.IdChu == updateHoaDon.idChu);
            HoaDon hoadon = db.HoaDons.FirstOrDefault(t => t.IdHoaDon == updateHoaDon.maHoaDon);
            Phong phong = db.Phongs.FirstOrDefault(t => t.IdPhong == hoadon.IdPhong);
            hoadon.TienDien = (updateHoaDon.soDienMoi - updateHoaDon.soDienCu);
            hoadon.TienNuoc = chu.GiaNuoc * phong.SoLuong;
            hoadon.TrangThai = 0;
            db.Entry(hoadon).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(new { message = "Cập nhật hóa đơn thành công" });
        }

        [HttpPut("XacNhanHoaDon/{id}")]
        public IActionResult XacNhanHoaDon(int id)
        {
            HoaDon hoadon = db.HoaDons.FirstOrDefault(t => t.IdHoaDon == id);
            if (hoadon == null)
            {
                return BadRequest(new { message = "Hóa đơn không tồn tại" });
            }
            hoadon.TrangThai = 1; // Đánh dấu hóa đơn đã thanh toán
            db.Entry(hoadon).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(new { message = "Xác nhận hóa đơn thành công" });
        }
    }
}