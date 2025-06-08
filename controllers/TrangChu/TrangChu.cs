using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

using api.Models.TrangChu;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.controllers.TrangChu
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrangChu : ControllerBase
    {
        MyDbContext db;
        public TrangChu(MyDbContext context)
        {
            db = context;
        }


        [HttpGet("GetThongTinChu/{id}")]
        public IActionResult GetThongTinChu(int id)
        {
            var chu = db.Chus.FirstOrDefault(t => t.IdChu == id);
            if (chu == null) return BadRequest(new { message = "Không có thông tin chủ cơ sở" });
            return Ok(new { chu.Ten, chu.TaiKhoan, chu.Avatar, chu.GiaNuoc, chu.GiaDien, chu.IdChu, chu.MatKhau });
        }


        [HttpGet("GetPhongvaKhach/{id}")]
        public IActionResult GetPhongvaKhach(int id)
        {
            List<int> listCoSo = db.CoSos.Where(u => u.IdChu == id).Select(t => t.IdCoSo).ToList();
            List<Phong> listPhong = db.Phongs.Where(t => listCoSo.Contains(t.IdCoSo) && t.TrangThai == 1).ToList();
            int tongPhong = listPhong.Count;
            int tongKhach = listPhong.Sum(t => t.SoLuong);
            int tongPhongDat = listPhong.Where(t => t.SoLuong > 0).Count();
            if (listCoSo == null) return Ok(new { tongPhong = 0, tongKhach = 0, tongPhongDat = 0 });
            if (listPhong == null) return Ok(new { tongPhong = 0, tongKhach = 0, tongPhongDat = 0 });
            return Ok(new { tongPhong, tongKhach, phongTrong = tongPhong - tongPhongDat });
        }

        [HttpPut("UpdateHoSoChu/{id}")]
        public IActionResult UpdateHoSoChu(int id, [FromBody] Updatechu Update)
        {
            Chu chu = db.Chus.FirstOrDefault(t => t.IdChu == id);
            if (chu == null)
                return NotFound(new { message = "Không tìm thấy chủ với id này" });

            chu.Ten = Update.hoten;
            chu.TaiKhoan = Update.taikhoan;
            chu.GiaDien = Update.giadien;
            chu.GiaNuoc = Update.gianuoc;
            db.Chus.Update(chu);
            db.SaveChanges();

            return Ok();
        }

        [HttpPut("UpdateAvatarChu/{id}")]
        public async Task<IActionResult> UpdateAvatarChu(int id, [FromBody] updateAvatar data)
        {
            Chu chu = db.Chus.FirstOrDefault(t => t.IdChu == id);
            if (chu == null)
                return NotFound(new { message = "Không tìm thấy chủ với id này" });

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
                string fileName = $"avatar_{id}_{DateTime.Now.Ticks}.png";
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Avatar");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                // Lưu đường dẫn (hoặc tên file) vào DB
                chu.Avatar = $"{fileName}";
                db.Chus.Update(chu);
                db.SaveChanges();
                return Ok(new { Message = "Cập nhật avatar thành công" });
            }
            catch (FormatException e)
            {
                return BadRequest(new { message = "Lỗi giải mã base64: " + e.Message });
            }




        }


        [HttpPut("updateMatKhau/{idChu}")]
        public IActionResult updateMatKhau(int idChu,[FromBody] updateMatKhau mkMoi)
        {

            Chu chu = db.Chus.FirstOrDefault(t => t.IdChu == idChu);
            if (chu == null)
                return NotFound(new { message = "Không tìm thấy chủ với id này" });
            if (string.IsNullOrEmpty(mkMoi.matkhauMoi))
                return BadRequest(new { message = "Mật khẩu mới không được để trống" });
            chu.MatKhau = mkMoi.matkhauMoi;
            db.Chus.Update(chu);
            db.SaveChanges();

            return Ok(new { message = "Cập nhật mật khẩu thành công" });

        }
    }
}