using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using api.Models;
using api.Models.QuanLyCoSo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.controllers.QuanLy
{
    [ApiController]
    [Route("api/[controller]")]
    public class QLCoSoVaPhong : ControllerBase
    {
        MyDbContext db;
        public QLCoSoVaPhong(MyDbContext context)
        {
            db = context;
        }

        [HttpGet("GetCoSo/{id}")]
        public IActionResult GetCoSo(int id)
        {
            var coSo = db.CoSos.Where(t => t.IdChu == id).Select(u=>new{u.IdCoSo,u.TenCoSo,u.SoLuong,u.DiaChi,u.IdChu,u.TrangThai}).ToList();
            if (coSo == null || coSo.Count == 0) return BadRequest(new { message = "Không có cơ sở nào" });
            return Ok(coSo);
        }

        [HttpPost("ThemCoSo/{id}")]
        public IActionResult ThemCoSo(int id, [FromBody] ThemCoSo themCoSo){

            if (themCoSo == null) return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            if (string.IsNullOrEmpty(themCoSo.TenCoSo) || string.IsNullOrEmpty(themCoSo.DiaChi)) return BadRequest(new { message = "Tên cơ sở và địa chỉ không được để trống" });
            CoSo check= db.CoSos.FirstOrDefault(t => (t.TenCoSo == themCoSo.TenCoSo) && t.IdChu == id);
            if (check != null) return BadRequest(new { message = "Cơ sở đã tồn tại" });
            CoSo coSo = new CoSo
            {
                IdChu = id,
                TenCoSo = themCoSo.TenCoSo,
                DiaChi = themCoSo.DiaChi
            };
            db.CoSos.Add(coSo);
            db.SaveChanges();
            return Ok();
        }


        [HttpDelete("XoaCoSo/{id}")]
        public IActionResult XoaCoSo(int id){

            var coSo = db.CoSos.FirstOrDefault(t => t.IdCoSo == id);
            if (coSo == null) return BadRequest(new { message = "Cơ sở không tồn tại" });
            db.CoSos.Remove(coSo);
            db.SaveChanges();
            return Ok(new { message = "Xóa cơ sở thành công" });
        }
      
        [HttpPut("CapNhatCoSo/{id}")]
        public IActionResult CapNhatCoSo(int id, [FromBody] ThemCoSo themCoSo)
        {
            if (themCoSo == null) return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            if (string.IsNullOrEmpty(themCoSo.TenCoSo) || string.IsNullOrEmpty(themCoSo.DiaChi)) return BadRequest(new { message = "Tên cơ sở và địa chỉ không được để trống" });
            CoSo coSo = db.CoSos.FirstOrDefault(t => t.IdCoSo == id);
            if (coSo == null) return BadRequest(new { message = "Cơ sở không tồn tại" });
            coSo.TenCoSo = themCoSo.TenCoSo;
            coSo.DiaChi = themCoSo.DiaChi;

            db.SaveChanges();
            return Ok(new { message = "Cập nhật cơ sở thành công" });
        }



        //-------------------------------------------Quản Lý Phòng----------------------------------------------

        
        [HttpGet("GetPhong/{id}")]
        public IActionResult GetPhong(int id)
        {
            var phong = db.Phongs.Where(t => t.IdCoSo == id&&t.TrangThai==1).Select(u => new { u.IdPhong, u.TenPhong,u.TienPhong, u.SoLuong, u.TrangThai,u.IdCoSo }).ToList();
            if (phong == null || phong.Count == 0) return BadRequest(new { message = "Không có phòng nào" });
            return Ok(phong);
        }


        [HttpPost("ThemPhong/{id}")]
        public IActionResult ThemPhong(int id,[FromBody] ThemPhong themPhong)
        {
            if (themPhong == null) return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            if (string.IsNullOrEmpty(themPhong.TenPhong)) return BadRequest(new { message = "Tên phòng không được để trống" });
            Phong check = db.Phongs.FirstOrDefault(t => t.TenPhong == themPhong.TenPhong && t.IdCoSo == id&&t.TrangThai==1);
            if (check != null) return BadRequest(new { message = "Phòng đã tồn tại" });
            Phong phong = new Phong
            {
                IdCoSo = id,
                TenPhong = themPhong.TenPhong,
                SoLuong = 0,
                TienPhong=themPhong.giaPhong
            };
            db.Phongs.Add(phong);
            db.SaveChanges();
            return Ok();
        }

        [HttpDelete("XoaPhong/{id}")]
        public IActionResult XoaPhong(int id)
        {
            Phong phong = db.Phongs.FirstOrDefault(t => t.IdPhong == id);
            if (phong == null) return BadRequest(new { message = "Phòng không tồn tại" });
            
            
            phong.TrangThai = 0;
            db.Entry(phong).State = EntityState.Modified;
            CoSo coSo = db.CoSos.FirstOrDefault(t => t.IdCoSo == phong.IdCoSo);
            if (coSo != null)
            {
                coSo.SoLuong = coSo.SoLuong - 1;
                db.Entry(coSo).State = EntityState.Modified;
            }
            db.SaveChanges();
            return Ok(new { message = "Xóa phòng thành công" });
        }

        [HttpGet("getInfoPhong/{id}")]
        public IActionResult getInfoPhong(int id)
        {
            var phong = db.Phongs.FirstOrDefault(t => t.IdPhong == id);
            if (phong == null) return BadRequest(new { message = "Phòng không tồn tại" });
            var khachHangs = db.KhachHangs.Where(t => t.IdPhong == id&&t.Tinhtrang==1).ToList();
            return Ok(new
            {
                phong.IdPhong,
                phong.TenPhong,
                phong.SoLuong,
                phong.TienPhong,
                khachHangs = khachHangs.Select(t => new {  t.TenKh, t.Sdt,t.NgayDen }).ToList()
            });
        }


        [HttpPut("CapNhatPhong/{idPhong}")]
        public IActionResult CapNhatPhong(int idPhong, [FromBody] ThemPhong themPhong)
        {
            if (themPhong == null) return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            if (string.IsNullOrEmpty(themPhong.TenPhong)) return BadRequest(new { message = "Tên phòng không được để trống" });
            Phong phong = db.Phongs.FirstOrDefault(t => t.IdPhong == idPhong);
            if (phong == null) return BadRequest(new { message = "Phòng không tồn tại" });
            Phong check = db.Phongs.FirstOrDefault(t => t.TenPhong == themPhong.TenPhong && t.IdPhong== idPhong);
            if (check != null) return BadRequest(new { message = "Tên phòng đã tồn tại" });
            phong.TenPhong = themPhong.TenPhong;
            db.SaveChanges();
            return Ok(new { message = "Cập nhật phòng thành công" });
        }

    }
}