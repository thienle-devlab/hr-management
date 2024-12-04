using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Thuong
{
    public string MaThuong { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public DateTime NgayThuong { get; set; }

    public int SoTienThuong { get; set; }

    public string? LyDoThuong { get; set; }

    public virtual Nhanvien? MaNhanVienNavigation { get; set; } = null!;
}
