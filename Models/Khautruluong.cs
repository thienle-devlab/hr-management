using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Khautruluong
{
    public string MaKhauTruLuong { get; set; } = null!;

    public string MaLuong { get; set; } = null!;

    public string MaKhauTru { get; set; } = null!;

    public int KhoanKhauTru { get; set; }

    public DateTime NgayApDung { get; set; }

    public virtual Khautru MaKhauTruNavigation { get; set; } = null!;

    public virtual Luong MaLuongNavigation { get; set; } = null!;
}
