using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.controllers.DangNhapDangKy
{
    [ApiController]
    [Route("api/[controller]")]
    public class dangnhap : ControllerBase
    {
        MyDbContext db;
        static DateTime han ;
        static string verificationCode = string.Empty;
        static int userIDforgotPassword = 0;



        public dangnhap(MyDbContext context)
        {
            db = context;
        }
        [HttpPost("DangKyChu")]
        public IActionResult DangKy([FromBody] DangKyChu user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user data.");
            }

            
            // Check if the username already exists
            var existingUser = db.Chus.FirstOrDefault(u => u.TaiKhoan == user.TaiKhoan);
            var existingKhachHang = db.KhachHangs.FirstOrDefault(u => u.Email == user.TaiKhoan);
            if (existingUser != null|| existingKhachHang != null)
            {
                return BadRequest("User already exists.");
            } 
            Chu chu=new Chu
            {
                Ten = user.Ten,
                TaiKhoan = user.TaiKhoan,
                MatKhau = user.MatKhau
            };
            db.Chus.Add(chu);
            db.SaveChanges();
            return Ok("User registered successfully.");
        }

        [HttpPost("DangNhapChu")]
        public IActionResult DangNhap([FromBody] DangNhapChu user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user data.");
            }

            // Check if the username and password are correct
            Chu existingUser = db.Chus.FirstOrDefault(u => u.TaiKhoan == user.TaiKhoan && u.MatKhau == user.MatKhau);
            KhachHang khachHang = db.KhachHangs.FirstOrDefault(u => u.Email == user.TaiKhoan && u.MatKhau == user.MatKhau);
            if (khachHang != null)
            {
                return Ok(new { idKH = khachHang.IdKh, message = "Đăng nhập thành công." });
            }
            if (existingUser != null)
            {
                if (existingUser.GiaDien == 0 || existingUser.GiaNuoc == 0)
                {
                    return Ok(new { id = existingUser.IdChu, message = "Chủ cơ sở chưa cập nhật giá điện hoặc giá nước." });
                }
                else
                {
                    return Ok(new { id = existingUser.IdChu });
                }
            }
            
            return Unauthorized("Invalid username or password.");
            

        }

        private void SendVerificationEmail(string email, string verificationCode)
        {
            var subject = "Mã xác thực để đặt lại mật khẩu";
            var body="";
            
            
            body = "Mã xác thực của bạn là:" + verificationCode + "  Vui lòng nhập mã này để đặt lại mật khẩu của bạn.";
            
            

        
            try
            {
               
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("vudinhdangkhoa03@gmail.com", "petr nlro qogc dhow"), 
                    EnableSsl = true // Bật mã hóa SSL
                };

                
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("vudinhdangkhoa03@gmail.com"), 
                    Subject = subject, 
                    Body = body, 
                    IsBodyHtml = false 
                };

                
                mail.To.Add(email);

               
                smtpClient.Send(mail);
               
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gửi email thất bại. Lỗi: " + ex.Message);
                
            }

        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }


        [HttpGet("ForgotPassword/{email}")]
        public IActionResult ForgotPassword(string email)
        {
            var user = db.Chus.FirstOrDefault(u => u.TaiKhoan == email);
            var khachHang = db.KhachHangs.FirstOrDefault(u => u.Email == email);
            if (khachHang != null)
            {
                userIDforgotPassword = khachHang.IdKh;
                verificationCode = GenerateVerificationCode();
                han = DateTime.Now.AddMinutes(30);

                SendVerificationEmail(email, verificationCode);
                return Ok();
            }
            if (user == null)
                return NotFound("No results found for this user.");

            else
            {
                userIDforgotPassword = user.IdChu;
                verificationCode = GenerateVerificationCode();
                han = DateTime.Now.AddMinutes(30);


                SendVerificationEmail(email, verificationCode);
            }
            return Ok();
        }


        [HttpGet("XacThuc/{verificationCode}")]
        public IActionResult XacThuc(string verificationCode)
        {
            if(string.IsNullOrEmpty(verificationCode))
            {
                return BadRequest("Mã xác thực không được để trống.");
            }
            if (verificationCode != verificationCode)
            {
                return BadRequest("Mã xác thực không đúng.");
            }
            if (DateTime.Now > han)
            {
                return BadRequest("Mã xác thực đã hết hạn.");
            }
            return Ok( new { UserId =userIDforgotPassword });
            
        }

        [HttpPut("CapLaiMatKhau/{userId}")]
        public IActionResult CapLaiMatKhau(int userId, [FromBody] DangNhapChu newPassword)
        {
            if (string.IsNullOrEmpty(newPassword.MatKhau))
            {
                return BadRequest("Mật khẩu không được để trống.");
            }
            
            var khachHang = db.KhachHangs.FirstOrDefault(u => u.IdKh == userId);
            if (khachHang != null)
            {
                khachHang.MatKhau = newPassword.MatKhau;
                db.SaveChanges();
                return Ok("Mật khẩu đã được đặt lại thành công.");
            }
            var user = db.Chus.FirstOrDefault(u => u.IdChu == userId);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }
            else
            {
                 user.MatKhau = newPassword.MatKhau;
                db.SaveChanges();
            }

           

            return Ok("Mật khẩu đã được đặt lại thành công.");
        }
        
    }

    
}