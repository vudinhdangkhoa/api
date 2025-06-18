using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models.QuanLyHoaDon;
using System.Net.Mail;
using System.Net;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font;
using iText.Kernel.Font;
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

        private byte[] TaoHoaDonPDF(HoaDon hoaDon, Phong phong, Chu chu)
{
    using (var stream = new MemoryStream())
    {
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // Load font Unicode, thay đường dẫn bằng font .ttf bạn có
        string fontPath = "C:/Windows/Fonts/tahoma.ttf"; // hoặc đường dẫn font phù hợp
        var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
        document.SetFont(font);

        // Gán giá trị mặc định nếu bị null
        double tienPhong = phong.TienPhong ?? 0;
        double tienDien = hoaDon.TienDien ?? 0;
        double giaDien = chu.GiaDien ?? 0;
        double tienNuoc = hoaDon.TienNuoc ?? 0;
        double giaNuoc = chu.GiaNuoc ?? 0;
        double tongTien = hoaDon.SoTien ?? (tienPhong + tienDien * giaDien + tienNuoc); // fallback nếu SoTien null
        DateOnly ngayTao = hoaDon.NgayTao ?? DateOnly.FromDateTime(DateTime.Now);

        var coso = db.CoSos.FirstOrDefault(cs => cs.IdCoSo == phong.IdCoSo);
        string tenCoSo = coso?.TenCoSo ?? "Không xác định";
        string tenPhong = phong?.TenPhong ?? "Không xác định";

        // Tiêu đề hóa đơn
        document.Add(new Paragraph("HÓA ĐƠN TIỀN PHÒNG")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(20));

        // Thông tin cơ bản
        document.Add(new Paragraph($"Cơ sở: {tenCoSo}").SetFontSize(12));
        document.Add(new Paragraph($"Phòng: {tenPhong}").SetFontSize(12));
        document.Add(new Paragraph($"Tháng: {ngayTao.Month}/{ngayTao.Year}").SetFontSize(12));

        var table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 2 })).UseAllAvailableWidth();
        table.AddHeaderCell("Khoản thu");
        table.AddHeaderCell("Đơn giá");
        table.AddHeaderCell("Thành tiền");

        table.AddCell("Tiền phòng");
        table.AddCell($"{tienPhong:N0} VNĐ");
        table.AddCell($"{tienPhong:N0} VNĐ");

        table.AddCell("Tiền điện");
        table.AddCell($"{giaDien:N0} VNĐ/kWh");
        table.AddCell($"{(tienDien * giaDien):N0} VNĐ");

        table.AddCell("Tiền nước");
        table.AddCell($"{giaNuoc:N0} VNĐ/người");
        table.AddCell($"{tienNuoc:N0} VNĐ");

        document.Add(table);

        double tongtien = tienPhong + (tienDien * giaDien) + tienNuoc;

        // Tổng cộng + ngày tạo
        document.Add(new Paragraph($"TỔNG CỘNG: {tongtien:N0} VNĐ")
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetFontSize(14));

        document.Add(new Paragraph($"Ngày tạo: {ngayTao:dd/MM/yyyy}")
            .SetTextAlignment(TextAlignment.RIGHT));

        document.Close();
        return stream.ToArray();
    }
}




        private void SendVerificationEmail(string email, HoaDon hoaDon, Phong phong, Chu chu)
        {
            var subject = $"Hóa đơn tháng {DateTime.Now.Month}/{DateTime.Now.Year}";
            var body = $"Xin chào,\n\nĐây là hóa đơn tháng {DateTime.Now.Month} của bạn.\nVui lòng kiểm tra file đính kèm.\n\nCảm ơn!";
            
            try
            {
                // Tạo file PDF

                byte[] pdfBytes ;
                try
                {
                    pdfBytes = TaoHoaDonPDF(hoaDon, phong, chu);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi tạo PDF: " + ex.ToString());
                    return;
                }
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("vudinhdangkhoa03@gmail.com", "petr nlro qogc dhow"),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("vudinhdangkhoa03@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                mail.To.Add(email);

                
                var attachment = new Attachment(new MemoryStream(pdfBytes), $"HoaDon_{DateTime.Now.Month}_{DateTime.Now.Year}.pdf", "application/pdf");
                mail.Attachments.Add(attachment);

                smtpClient.Send(mail);
                Console.WriteLine("Email đã được gửi thành công đến " + email);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gửi email thất bại. Lỗi: " + ex.Message);
            }
        }

        [HttpPut("XacNhanHoaDon/{id}")]
        public IActionResult XacNhanHoaDon(int id)
        {
            HoaDon hoadon = db.HoaDons.FirstOrDefault(t => t.IdHoaDon == id);
            if (hoadon == null)
            {
                return BadRequest(new { message = "Hóa đơn không tồn tại" });
            }

            Phong phong = db.Phongs.FirstOrDefault(t => t.IdPhong == hoadon.IdPhong);
            Chu chu = db.Chus.FirstOrDefault(c => c.IdChu == db.CoSos.FirstOrDefault(cs => cs.IdCoSo == phong.IdCoSo).IdChu);
            List<KhachHang> khachHangs = db.KhachHangs.Where(t => t.IdPhong == phong.IdPhong).ToList();

            foreach (KhachHang kh in khachHangs)
            {
                SendVerificationEmail(kh.Email, hoadon, phong, chu); // Gửi email kèm PDF
            }

            hoadon.TrangThai = 1;
            hoadon.NgayThanhToan = DateOnly.FromDateTime(DateTime.Now);
            db.Entry(hoadon).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(new { message = "Xác nhận hóa đơn thành công" });
        }
    }
}