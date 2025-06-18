using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.QuanLyKH;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.controllers.QuanLy
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuanLyKH : ControllerBase
    {
        MyDbContext db;
        public QuanLyKH(MyDbContext context)
        {
            db = context;
        }

        [HttpGet("GetKhach/{idChu}")]
        public IActionResult GetKhach(int idChu){
            
            var khach= db.KhachHangs.Join(db.Phongs,kh=>kh.IdPhong,p=>p.IdPhong,(kh,p)=>new {kh,p}).
            Join(db.CoSos,kh=>kh.p.IdCoSo,cs=>cs.IdCoSo,(kh,cs)=>new{kh,cs})
            .Join(db.Chus,kh=>kh.cs.IdChu,chu=>chu.IdChu,(kh,chu)=>new{kh,chu})
            .Where(t=>t.chu.IdChu==idChu).Select(data=> new{
                data.kh.kh.kh.IdKh,
                data.kh.kh.kh.TenKh,
                data.kh.kh.kh.Sdt,
                data.kh.kh.kh.IdPhong,
                data.kh.kh.p.TenPhong,
                data.kh.kh.kh.NgayDen,
                data.kh.kh.kh.Tinhtrang,
                data.kh.cs.TenCoSo,
                data.kh.cs.IdCoSo,
            });
            var coso=db.CoSos.Where(t=>t.IdChu==idChu).Select(u=>new{u.IdCoSo,u.TenCoSo,u.SoLuong,u.DiaChi,u.IdChu,u.TrangThai}).ToList();
            List<int> idCoSo = db.CoSos.Select(t => t.IdCoSo).ToList();
            var phongs = db.Phongs.Where(t => idCoSo.Contains(t.IdCoSo)&&t.TrangThai==1).Select(u=>new{ u.IdPhong, u.TenPhong, u.SoLuong, u.TrangThai,u.IdCoSo}).ToList();
            

            return Ok(new { khach, coso, phongs });
        }

        [HttpPut("deleteKhach/{idKh}")]
        public IActionResult deleteKhach(int idKh)
        {
            KhachHang khach = db.KhachHangs.FirstOrDefault(x => x.IdKh == idKh);
            if (khach == null)
            {
                return NotFound();
            }

            khach.Tinhtrang = 0;
            Phong phong = db.Phongs.FirstOrDefault(x => x.IdPhong == khach.IdPhong);
            if (phong != null)
            {
                phong.SoLuong = phong.SoLuong - 1;
                db.Entry(phong).State = EntityState.Modified;
            }
            db.Entry(khach).State = EntityState.Modified;
            db.SaveChanges();

            return Ok();
        }


        [HttpPost("ThemKhach/{idPhong}")]
        public IActionResult ThemKhach(int idPhong,[FromBody] ThemKH themkh){

            if (themkh == null) return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            if (string.IsNullOrEmpty(themkh.TenKh)) return BadRequest(new { message = "Tên khách không được để trống" });
            if (string.IsNullOrEmpty(themkh.Sdt)) return BadRequest(new { message = "Số điện thoại không được để trống" });
            if (string.IsNullOrEmpty(themkh.Cccd)) return BadRequest(new { message = "CCCD không được để trống" });

            KhachHang check = db.KhachHangs.FirstOrDefault(t => t.Sdt == themkh.Sdt && t.IdPhong == idPhong&&t.Tinhtrang==1);
            if (check != null) return BadRequest(new { message = "Khách đã tồn tại" });

            Phong phong = db.Phongs.FirstOrDefault(t => t.IdPhong == idPhong);
            if (phong == null) return BadRequest(new { message = "Phòng không tồn tại" });

            KhachHang a =new KhachHang
            {
                TenKh = themkh.TenKh,
                Sdt = themkh.Sdt,
                Cccd = themkh.Cccd,
                IdPhong = idPhong,
                NgayDen = DateOnly.FromDateTime(DateTime.Now),
                Tinhtrang = 1,
                Avatar= "khonghinh",
                Email = themkh.Email,
            };
            db.KhachHangs.Add(a);
            db.SaveChanges();
            return Ok();
        }
    }
}