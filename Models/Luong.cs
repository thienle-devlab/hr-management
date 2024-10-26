using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Luong
{
    public string MaLuong { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public int LuongCoBan { get; set; }

    public DateTime NgayNhanLuong { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<Khautruluong> Khautruluongs { get; set; } = new List<Khautruluong>();

    public virtual ICollection<Luongphucap> Luongphucaps { get; set; } = new List<Luongphucap>();

    public virtual Nhanvien MaNhanVienNavigation { get; set; } = null!;

    public virtual ICollection<Phat> Phats { get; set; } = new List<Phat>();

    public virtual ICollection<Thuong> Thuongs { get; set; } = new List<Thuong>();
}
