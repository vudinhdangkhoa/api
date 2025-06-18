using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.TrangChu;
using Microsoft.AspNetCore.Mvc;
using api.Models.Khach;
using Microsoft.EntityFrameworkCore;

namespace api.controllers.GiaoDienKhachHang
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrangChuKH : ControllerBase
    {
        MyDbContext db;
        public TrangChuKH(MyDbContext context)
        {
            db = context;
        }

        



        [HttpGet("getinfoKH/{idKh}")]
        public IActionResult getinfoKH(int idKh)
        {
            var khach = db.KhachHangs.Join(db.Phongs, kh => kh.IdPhong, p => p.IdPhong, (kh, p) => new { kh, p })

                .Join(db.CoSos, kh => kh.p.IdCoSo, cs => cs.IdCoSo, (kh, cs) => new { kh, cs })
                .Join(db.Chus, kh => kh.cs.IdChu, chu => chu.IdChu, (kh, chu) => new { kh, chu })
                .Where(t => t.kh.kh.kh.IdKh == idKh)
                .Select(data => new
                {
                    data.kh.kh.kh.IdKh,
                    data.kh.kh.kh.TenKh,
                    data.kh.kh.kh.Sdt,
                    data.kh.kh.kh.Email,
                    data.kh.kh.kh.MatKhau,
                    data.kh.kh.kh.Avatar,
                    data.kh.kh.p.TenPhong,
                    data.kh.cs.TenCoSo,
                    data.kh.kh.kh.Cccd,
                    data.chu.Ten,
                    data.chu.TaiKhoan,

                    data.chu.GiaNuoc,
                    data.chu.GiaDien,
                    data.kh.kh.kh.NgayDen,
                    data.kh.kh.p.SoLuong,
                    data.kh.kh.p.TienPhong,
                    data.kh.cs.DiaChi,
                    soDien = db.HoaDons.Where(t => t.IdPhong == data.kh.kh.p.IdPhong && t.NgayThanhToan.Value.Month == DateTime.Now.Month - 1 && t.NgayThanhToan.Value.Year == DateTime.Now.Year).Select(t => t.TienDien).FirstOrDefault(),
                }).FirstOrDefault();
            if (khach == null)
            {
                return NotFound("Khách hàng không tồn tại.");
            }
            return Ok(khach);
        }


        [HttpPut("updateMatKhauKH/{idKh}")]
        public IActionResult updatematKhauKH(int idKh, [FromBody] updateMatKhau MKMoi)
        {

            KhachHang khach = db.KhachHangs.FirstOrDefault(kh => kh.IdKh == idKh);
            if (khach == null)
            {
                return NotFound("Khách hàng không tồn tại.");

            }
            if (string.IsNullOrEmpty(MKMoi.matkhauMoi))
            {
                return BadRequest("Mật khẩu mới không được để trống.");
            }

            khach.MatKhau = MKMoi.matkhauMoi;
            db.KhachHangs.Update(khach);
            db.SaveChanges();

            return Ok("Cập nhật mật khẩu thành công");

        }


        [HttpPut("updateAvatarKH/{idKh}")]
        public async Task<IActionResult> updateAvatarKH(int idKh, [FromBody] updateAvatar data)
        {

            KhachHang khach = db.KhachHangs.FirstOrDefault(kh => kh.IdKh == idKh);
            if (khach == null)
            {
                return NotFound("Khách hàng không tồn tại.");
            }

            string base64Image = data.avatar;
            if (string.IsNullOrEmpty(base64Image))
                return BadRequest(new { message = "Không có dữ liệu ảnh" });

            // Giải mã base64
            try
            {
                // KHÔNG trim, chỉ remove line breaks nếu cần
                string cleanedBase64 = base64Image.Replace("\r", "").Replace("\n", "");
                byte[] imageBytes = Convert.FromBase64String(cleanedBase64);
                // tiếp tục xử lý
                // Tạo tên file duy nhất
                string fileName = $"avatarKH_{idKh}_{DateTime.Now.Ticks}.png";
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Avatar");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                // Lưu đường dẫn (hoặc tên file) vào DB
                khach.Avatar = $"{fileName}";
                db.KhachHangs.Update(khach);
                db.SaveChanges();
                return Ok(new { Message = "Cập nhật avatar thành công" });
            }
            catch (FormatException e)
            {
                return BadRequest(new { message = "Lỗi giải mã base64: " + e.Message });
            }
        }

        [HttpPut("updateInfoKH/{idKh}")]
        public IActionResult updateInfoKH(int idKh, [FromBody] updateInfoKH info)
        {
            KhachHang khach = db.KhachHangs.FirstOrDefault(kh => kh.IdKh == idKh);
            if (khach == null)
            {
                return NotFound("Khách hàng không tồn tại.");
            }

            if (string.IsNullOrEmpty(info.tenKH) || string.IsNullOrEmpty(info.sdt) || string.IsNullOrEmpty(info.email))
            {
                return BadRequest("Thông tin không được để trống.");
            }

            khach.TenKh = info.tenKH;
            khach.Sdt = info.sdt;
            khach.Email = info.email;
            khach.Cccd = info.cccd;
            db.KhachHangs.Update(khach);
            db.SaveChanges();

            return Ok("Cập nhật thông tin thành công");
        }

        [HttpGet("getHoaDon/{idKh}")]
        public IActionResult getHoaDon(int idKh)
        {
            var khach = db.KhachHangs.FirstOrDefault(kh => kh.IdKh == idKh);
            if (khach == null)
            {
                return NotFound("Khách hàng không tồn tại.");
            }

            var HoaDons = db.HoaDons.Join(db.Phongs, hd => hd.IdPhong, p => p.IdPhong, (hd, p) => new { p, hd })
                .Join(db.KhachHangs, p => p.hd.IdPhong, kh => kh.IdPhong, (p, kh) => new { p, kh })
                .Where(t => t.kh.IdKh == idKh && t.p.hd.NgayTao.Value.Month == DateTime.Now.Month && t.p.hd.NgayTao.Value.Year == DateTime.Now.Year && t.p.hd.TrangThai != -1)

                .Select(data => new
                {
                    data.p.hd.IdHoaDon,
                    data.p.hd.SoTien,
                    data.p.hd.TienDien,
                    data.p.hd.TienNuoc,
                    data.p.hd.NgayTao,
                    data.p.hd.NgayThanhToan,
                    data.p.hd.TrangThai,
                    data.p.hd.AnhHoaDon,
                    data.p.p.TenPhong,
                    data.kh.TenKh,
                    data.kh.Sdt
                }).FirstOrDefault();

            return Ok(HoaDons);

        }

        [HttpPut("uploadPaymentImage/{idHD}")]
        public IActionResult uploadPaymentImage(int idHD, [FromBody] updateAvatar image)
        {
            var hoaDon = db.HoaDons.FirstOrDefault(hd => hd.IdHoaDon == idHD);
            if (hoaDon == null)
            {
                return NotFound("Hóa đơn không tồn tại.");
            }

            if (string.IsNullOrEmpty(image.avatar))
            {
                return BadRequest("Ảnh hóa đơn không được để trống.");
            }

            // Giải mã base64
            try
            {
                string cleanedBase64 = image.avatar.Replace("\r", "").Replace("\n", "");
                byte[] imageBytes = Convert.FromBase64String(cleanedBase64);

                // Tạo tên file duy nhất
                string fileName = $"hoaDon_{idHD}_{DateTime.Now.Ticks}.png";
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "HoaDon");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, fileName);
                System.IO.File.WriteAllBytes(filePath, imageBytes);

                // Lưu đường dẫn vào DB
                hoaDon.AnhHoaDon = $"{fileName}";
                hoaDon.TrangThai = 2;
                db.HoaDons.Update(hoaDon);
                db.SaveChanges();

                return Ok(new { Message = "Cập nhật ảnh hóa đơn thành công" });
            }
            catch (FormatException e)
            {
                return BadRequest(new { message = "Lỗi giải mã base64: " + e.Message });
            }
        }

    }
}