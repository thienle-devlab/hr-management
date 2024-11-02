using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Models;

public partial class HrmContext : DbContext
{
    public HrmContext()
    {
    }

    public HrmContext(DbContextOptions<HrmContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Baocao> Baocaos { get; set; }

    public virtual DbSet<Chamcong> Chamcongs { get; set; }

    public virtual DbSet<Chucvu> Chucvus { get; set; }

    public virtual DbSet<Congviec> Congviecs { get; set; }

    public virtual DbSet<Khautru> Khautrus { get; set; }

    public virtual DbSet<Khautruluong> Khautruluongs { get; set; }

    public virtual DbSet<Luong> Luongs { get; set; }

    public virtual DbSet<Luongphucap> Luongphucaps { get; set; }

    public virtual DbSet<Nghiphep> Nghipheps { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Phancong> Phancongs { get; set; }

    public virtual DbSet<Phat> Phats { get; set; }

    public virtual DbSet<Phongban> Phongbans { get; set; }

    public virtual DbSet<Phucap> Phucaps { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public virtual DbSet<Thuong> Thuongs { get; set; }

    public DbSet<Hopdong> HopDongs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-0SBNK63\\SQLEXPRESS01;Initial Catalog=HRM;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Baocao>(entity =>
        {
            entity.HasKey(e => e.MaBaoCao).HasName("PK__BAOCAO__25A9188C5B7DE56C");

            entity.ToTable("BAOCAO");

            entity.Property(e => e.MaBaoCao)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DuLieuBaoCao).HasColumnType("ntext");
            entity.Property(e => e.LoaiBaoCao).HasMaxLength(50);
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayTaoBaoCao).HasColumnType("datetime");
            entity.Property(e => e.TenBaoCao).HasMaxLength(50);
            entity.Property(e => e.TrangThaiBaoCao).HasMaxLength(20);

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.Baocaos)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BC_NV");
        });

        modelBuilder.Entity<Chamcong>(entity =>
        {
            entity.HasKey(e => e.MaChamCong).HasName("PK__CHAMCONG__307331A18DDF1E50");

            entity.ToTable("CHAMCONG");

            entity.Property(e => e.MaChamCong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CheckIn).HasColumnType("datetime");
            entity.Property(e => e.CheckOut).HasColumnType("datetime");
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayChamCong).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.Chamcongs)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CC_NV");
        });

        modelBuilder.Entity<Chucvu>(entity =>
        {
            entity.HasKey(e => e.MaChucVu).HasName("PK__CHUCVU__D4639533A2099E28");

            entity.ToTable("CHUCVU");

            entity.Property(e => e.MaChucVu)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaPhongBan)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenChucVu).HasMaxLength(100);

            entity.HasOne(d => d.MaPhongBanNavigation).WithMany(p => p.Chucvus)
                .HasForeignKey(d => d.MaPhongBan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CV_PB");
        });

        modelBuilder.Entity<Congviec>(entity =>
        {
            entity.HasKey(e => e.MaCongViec).HasName("PK__CONGVIEC__41B7DD18EF670E57");

            entity.ToTable("CONGVIEC");

            entity.Property(e => e.MaCongViec)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MoTaCongViec).HasColumnType("ntext");
            entity.Property(e => e.TenCongViec).HasMaxLength(50);
        });

        modelBuilder.Entity<Khautru>(entity =>
        {
            entity.HasKey(e => e.MaKhauTru).HasName("PK__KHAUTRU__B19C10BB97EAFE8A");

            entity.ToTable("KHAUTRU");

            entity.Property(e => e.MaKhauTru)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenKhauTru).HasMaxLength(50);
        });

        modelBuilder.Entity<Khautruluong>(entity =>
        {
            entity.HasKey(e => e.MaKhauTruLuong).HasName("PK__KHAUTRUL__E88839032202BA57");

            entity.ToTable("KHAUTRULUONG");

            entity.Property(e => e.MaKhauTruLuong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaKhauTru)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaLuong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayApDung).HasColumnType("datetime");

            entity.HasOne(d => d.MaKhauTruNavigation).WithMany(p => p.Khautruluongs)
                .HasForeignKey(d => d.MaKhauTru)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KTL_KT");

            entity.HasOne(d => d.MaLuongNavigation).WithMany(p => p.Khautruluongs)
                .HasForeignKey(d => d.MaLuong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KTL_L");
        });

        modelBuilder.Entity<Luong>(entity =>
        {
            entity.HasKey(e => e.MaLuong).HasName("PK__LUONG__6609A48DD7BCC558");

            entity.ToTable("LUONG");

            entity.Property(e => e.MaLuong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayNhanLuong).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.Luongs)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_L_NV");
        });

        modelBuilder.Entity<Luongphucap>(entity =>
        {
            entity.HasKey(e => e.MaLuongPhuCap).HasName("PK__LUONGPHU__735D9D6764EE17C8");

            entity.ToTable("LUONGPHUCAP");

            entity.Property(e => e.MaLuongPhuCap)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaLuong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaPhuCap)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayApDung).HasColumnType("datetime");

            entity.HasOne(d => d.MaLuongNavigation).WithMany(p => p.Luongphucaps)
                .HasForeignKey(d => d.MaLuong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LPC_L");

            entity.HasOne(d => d.MaPhuCapNavigation).WithMany(p => p.Luongphucaps)
                .HasForeignKey(d => d.MaPhuCap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LPC_PC");
        });

        modelBuilder.Entity<Nghiphep>(entity =>
        {
            entity.HasKey(e => e.MaNghiPhep).HasName("PK__NGHIPHEP__1ED7CDB4BD94D157");

            entity.ToTable("NGHIPHEP");

            entity.Property(e => e.MaNghiPhep)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.LyDo).HasColumnType("ntext");
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.Nghipheps)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NP_NV");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NHANVIEN__77B2CA4719FD023B");

            entity.ToTable("NHANVIEN");

            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MaChucVu)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenNhanVien).HasMaxLength(100);

            entity.HasOne(d => d.MaChucVuNavigation).WithMany(p => p.Nhanviens)
                .HasForeignKey(d => d.MaChucVu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NV_CV");
        });

        modelBuilder.Entity<Phancong>(entity =>
        {
            entity.HasKey(e => e.MaPhanCong).HasName("PK__PHANCONG__C279D91611211C01");

            entity.ToTable("PHANCONG");

            entity.Property(e => e.MaPhanCong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaCongViec)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.Phancongs)
                .HasForeignKey(d => d.MaCongViec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PC_CV");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.Phancongs)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PC_NV");
        });

        modelBuilder.Entity<Phat>(entity =>
        {
            entity.HasKey(e => e.MaPhat).HasName("PK__PHAT__4AC072E20B5D2876");

            entity.ToTable("PHAT");

            entity.Property(e => e.MaPhat)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.LyDoPhat).HasColumnType("ntext");
            entity.Property(e => e.MaLuong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayPhat).HasColumnType("datetime");

            entity.HasOne(d => d.MaLuongNavigation).WithMany(p => p.Phats)
                .HasForeignKey(d => d.MaLuong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_P_L");
        });

        modelBuilder.Entity<Phongban>(entity =>
        {
            entity.HasKey(e => e.MaPhongBan).HasName("PK__PHONGBAN__D0910CC8D68F8DD0");

            entity.ToTable("PHONGBAN");

            entity.Property(e => e.MaPhongBan)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenPhongBan).HasMaxLength(100);
        });

        modelBuilder.Entity<Phucap>(entity =>
        {
            entity.HasKey(e => e.MaPhuCap).HasName("PK__PHUCAP__F97D973833EAD57F");

            entity.ToTable("PHUCAP");

            entity.Property(e => e.MaPhuCap)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenPhuCap).HasMaxLength(50);
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__TAIKHOAN__536C85E5D984BEBD");

            entity.ToTable("TAIKHOAN");

            entity.HasIndex(e => e.MaNhanVien, "UQ__TAIKHOAN__77B2CA4624BF6A51").IsUnique();

            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.MaNhanVienNavigation).WithOne(p => p.Taikhoan)
                .HasForeignKey<Taikhoan>(d => d.MaNhanVien)
                .HasConstraintName("FK_TK_NV");
        });

        modelBuilder.Entity<Thuong>(entity =>
        {
            entity.HasKey(e => e.MaThuong).HasName("PK__THUONG__5353CF4328103F33");

            entity.ToTable("THUONG");

            entity.Property(e => e.MaThuong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.LyDoThuong).HasColumnType("ntext");
            entity.Property(e => e.MaLuong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayThuong).HasColumnType("datetime");

            entity.HasOne(d => d.MaLuongNavigation).WithMany(p => p.Thuongs)
                .HasForeignKey(d => d.MaLuong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_T_L");
        });

        modelBuilder.Entity<Hopdong>(entity =>
        {
            entity.HasKey(e => e.MaHopDong)
                .HasName("PK_HOPDONG");

            entity.ToTable("HOPDONG");

            entity.Property(e => e.MaHopDong)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.NgayKy)
                .HasColumnType("datetime");

            entity.Property(e => e.NgayBatDau)
                .HasColumnType("datetime");

            entity.Property(e => e.NgayKetThuc)
                .HasColumnType("datetime");

            entity.Property(e => e.LoaiHopDong)
                .HasMaxLength(50);

            entity.Property(e => e.MucLuong)
                .HasColumnType("int");

            entity.Property(e => e.TrangThai)
                .HasMaxLength(20);

            entity.HasOne(d => d.MaNhanVienNavigation) // Quan hệ với bảng NHANVIEN
                .WithMany(p => p.HopDongs) // Giả sử bạn đã thêm DbSet<HopDong> trong model NHANVIEN
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull) // Chọn hành vi xóa
                .HasConstraintName("FK_HD_NV");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
