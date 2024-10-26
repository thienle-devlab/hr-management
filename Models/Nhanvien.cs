using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Nhanvien
{
    public string MaNhanVien { get; set; } = null!;

    public string TenNhanVien { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    public bool GioiTinh { get; set; }

    public string AnhDaiDien { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string Cccd { get; set; } = null!;

    public string MaChucVu { get; set; } = null!;

    public virtual ICollection<Baocao> Baocaos { get; set; } = new List<Baocao>();

    public virtual ICollection<Chamcong> Chamcongs { get; set; } = new List<Chamcong>();

    public virtual ICollection<Luong> Luongs { get; set; } = new List<Luong>();

    public virtual Chucvu MaChucVuNavigation { get; set; } = null!;

    public virtual ICollection<Nghiphep> Nghipheps { get; set; } = new List<Nghiphep>();

    public virtual ICollection<Phancong> Phancongs { get; set; } = new List<Phancong>();

    public virtual Taikhoan? Taikhoan { get; set; }
}
