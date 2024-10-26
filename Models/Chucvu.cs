using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Chucvu
{
    public string MaChucVu { get; set; } = null!;

    public string TenChucVu { get; set; } = null!;

    public string MaPhongBan { get; set; } = null!;

    public virtual Phongban MaPhongBanNavigation { get; set; } = null!;

    public virtual ICollection<Nhanvien> Nhanviens { get; set; } = new List<Nhanvien>();
}
