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
    public class ThongKeHoaDon : ControllerBase
    {
        MyDbContext db;
        public ThongKeHoaDon(MyDbContext context)
        {
            db = context;
        }


        [HttpGet("GetTrangThaiHoaDon/{idChu}")]
        public IActionResult GetTrangThaiHoaDon(int idChu)
        {
            var trangThaiHoaDon = db.HoaDons.Join(db.Phongs, hd => hd.IdPhong, p => p.IdPhong, (hd, p) => new { hd, p })
                .Join(db.CoSos, hd_p => hd_p.p.IdCoSo, cs => cs.IdCoSo, (hd_p, cs) => new { hd_p, cs })
                .Join(db.Chus, hd_p_cs => hd_p_cs.cs.IdChu, chu => chu.IdChu, (hd_p_cs, chu) => new { hd_p_cs, chu })
                .Where(t => t.chu.IdChu == idChu)
                .GroupBy(
                    data=> new
                    {
                        data.hd_p_cs.hd_p.hd.TrangThai,
                        
                    }
                )
                .Select(t => new
                {
                    t.Key.TrangThai,
                    soluong=t.Count()
                })
                .ToList();
            return Ok(trangThaiHoaDon);

        }

    }
}