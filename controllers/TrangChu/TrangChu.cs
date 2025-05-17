using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Mvc;

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
        
        [HttpGet("GetPhongvaKhach/{id}")]
        public IActionResult GetPhongvaKhach(int id){
            List<int> listCoSo = db.CoSos.Where(u=>u.IdChu==id).Select(t=>t.IdCoSo).ToList();
            List<Phong> listPhong = db.Phongs.Where(t=>listCoSo.Contains(t.IdCoSo)&&t.TrangThai==1).ToList();
            int tongPhong = listPhong.Count;
            int tongKhach = listPhong.Sum(t=>t.SoLuong);
            int tongPhongDat=listPhong.Where(t=>t.SoLuong>0).Count();
            if(listCoSo==null) return Ok(new{tongPhong=0,tongKhach=0,tongPhongDat=0});
            if(listPhong==null) return Ok(new{tongPhong=0,tongKhach=0,tongPhongDat=0});
            return Ok(new {tongPhong,tongKhach,phongTrong=tongPhong-tongPhongDat});
        }

        

    }
}