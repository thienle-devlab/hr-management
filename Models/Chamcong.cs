using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Chamcong
{
    public string MaChamCong { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public DateTime NgayChamCong { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual Nhanvien MaNhanVienNavigation { get; set; } = null!;
}
