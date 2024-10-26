using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Phancong
{
    public string MaPhanCong { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public string MaCongViec { get; set; } = null!;

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual Congviec MaCongViecNavigation { get; set; } = null!;

    public virtual Nhanvien MaNhanVienNavigation { get; set; } = null!;
}
