using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Nghiphep
{
    public string MaNghiPhep { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public string LyDo { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual Nhanvien MaNhanVienNavigation { get; set; } = null!;
}
