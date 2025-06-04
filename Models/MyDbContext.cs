using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Chu> Chus { get; set; }

    public virtual DbSet<CoSo> CoSos { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=KUPHA;Database=QLPhongTro;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.IdChat).HasName("PK__Chat__8307BCB3C2EB9DA8");

            entity.ToTable("Chat");

            entity.Property(e => e.IdChat).HasColumnName("idChat");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.IdKh).HasColumnName("idKH");
            entity.Property(e => e.NameChat)
                .HasMaxLength(50)
                .HasColumnName("nameChat");

            entity.HasOne(d => d.IdKhNavigation).WithMany(p => p.Chats)
                .HasForeignKey(d => d.IdKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chat_KhachHang");
        });

        modelBuilder.Entity<Chu>(entity =>
        {
            entity.HasKey(e => e.IdChu).HasName("PK__Chu__398F47B22851F818");

            entity.ToTable("Chu");

            entity.Property(e => e.IdChu).HasColumnName("idChu");
            entity.Property(e => e.Avatar)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("khonghinh")
                .HasColumnName("avatar");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("matKhau");
            entity.Property(e => e.TaiKhoan)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("taiKhoan");
            entity.Property(e => e.Ten)
                .HasMaxLength(50)
                .HasColumnName("ten");
        });

        modelBuilder.Entity<CoSo>(entity =>
        {
            entity.HasKey(e => e.IdCoSo).HasName("PK__CoSo__80B08911DEA382AB");

            entity.ToTable("CoSo");

            entity.Property(e => e.IdCoSo).HasColumnName("idCoSo");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("diaChi");
            entity.Property(e => e.IdChu).HasColumnName("idChu");
            entity.Property(e => e.SoLuong)
                .HasDefaultValue(0)
                .HasColumnName("soLuong");
            entity.Property(e => e.TenCoSo)
                .HasMaxLength(50)
                .HasColumnName("tenCoSo");
            entity.Property(e => e.TrangThai)
                .HasDefaultValue(1)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.IdChuNavigation).WithMany(p => p.CoSos)
                .HasForeignKey(d => d.IdChu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CoSo__idChu__398D8EEE");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.IdHoaDon).HasName("PK__HoaDon__B060C52C70E8EEFA");

            entity.ToTable("HoaDon");

            entity.Property(e => e.IdHoaDon).HasColumnName("idHoaDon");
            entity.Property(e => e.IdPhong).HasColumnName("idPhong");
            entity.Property(e => e.NgayThanhToan).HasColumnName("ngayThanhToan");
            entity.Property(e => e.SoTien).HasColumnName("soTien");
            entity.Property(e => e.TienDien).HasColumnName("tienDien");
            entity.Property(e => e.TienNuoc).HasColumnName("tienNuoc");
            entity.Property(e => e.TrangThai)
                .HasDefaultValue(0)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.IdPhongNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.IdPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__idPhong__44FF419A");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.IdKh).HasName("PK__KhachHan__9DB871740FB30303");

            entity.ToTable("KhachHang", tb =>
                {
                    tb.HasTrigger("trg_InsertKhachHang");
                    tb.HasTrigger("trg_deleteKhachHang");
                });

            entity.Property(e => e.IdKh).HasColumnName("idKH");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cccd");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IdPhong).HasColumnName("idPhong");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("123")
                .HasColumnName("matKhau");
            entity.Property(e => e.NgayDen)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("ngayDen");
            entity.Property(e => e.NgayDi).HasColumnName("ngayDi");
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("sdt");
            entity.Property(e => e.TenKh)
                .HasMaxLength(255)
                .HasColumnName("tenKH");
            entity.Property(e => e.Tinhtrang).HasColumnName("tinhtrang");

            entity.HasOne(d => d.IdPhongNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.IdPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__idPho__4222D4EF");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.IdMess).HasName("PK__Message__C26D1DE18AFF8429");

            entity.ToTable("Message");

            entity.Property(e => e.IdMess).HasColumnName("idMess");
            entity.Property(e => e.IdChat).HasColumnName("idChat");
            entity.Property(e => e.NguoiGui)
                .HasMaxLength(50)
                .HasColumnName("nguoiGui");
            entity.Property(e => e.NoiDung).HasColumnName("noiDung");

            entity.HasOne(d => d.IdChatNavigation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.IdChat)
                .HasConstraintName("FK__Message__idChat__49C3F6B7");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.IdPhong).HasName("PK__Phong__E540EED41A9543E4");

            entity.ToTable("Phong", tb =>
                {
                    tb.HasTrigger("trg_deletePhong");
                    tb.HasTrigger("trg_insertCoSo");
                });

            entity.Property(e => e.IdPhong).HasColumnName("idPhong");
            entity.Property(e => e.IdCoSo).HasColumnName("idCoSo");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");
            entity.Property(e => e.TenPhong)
                .HasMaxLength(50)
                .HasColumnName("tenPhong");
            entity.Property(e => e.TrangThai)
                .HasDefaultValue(1)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.IdCoSoNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.IdCoSo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Phong__idCoSo__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
