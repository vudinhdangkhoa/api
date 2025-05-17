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
    public class ThongKeKhach : ControllerBase
    {
        MyDbContext db;
        public ThongKeKhach(MyDbContext context)
        {
            db = context;
        }

        [HttpGet("GetKhach/{idChu}")]
        public IActionResult GetSLPhongVaKH(int idChu){
            
          
            var coSo = db.CoSos.Where(t => t.IdChu == idChu)
                .Select(u => new
                {
                    u.IdCoSo,
                    u.TenCoSo,
                    SoPhong = db.Phongs.Count(p => p.IdCoSo == u.IdCoSo && p.TrangThai == 1),
                    // SoKhach = db.KhachHangs.Join(db.Phongs, kh => kh.IdPhong, p => p.IdPhong, (kh, p) => new { kh, p })
                    //     .Where(kh => kh.p.IdCoSo == u.IdCoSo && kh.kh.Tinhtrang == 1)
                    //     .Count()
                    SoKhach=db.Phongs.Where(t=>t.IdCoSo==u.IdCoSo&&t.TrangThai==1).Sum(t=> t.SoLuong)
                })
                .ToList();


            return Ok(new{coSo,phongthue=db.Phongs.Join(db.CoSos, p => p.IdCoSo, cs => cs.IdCoSo, (p, cs) => new { p, cs })
                .Where(t => t.p.TrangThai == 1 && t.cs.IdChu == idChu&&t.p.SoLuong>0).Count(),phongtrong=db.Phongs.Join(db.CoSos, p => p.IdCoSo, cs => cs.IdCoSo, (p, cs) => new { p, cs })
                .Where(t => t.p.TrangThai == 1 && t.cs.IdChu == idChu&&t.p.SoLuong==0).Count()});
        }

    }
}