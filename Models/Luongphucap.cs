using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Luongphucap
{
    public string MaLuongPhuCap { get; set; } = null!;

    public string MaLuong { get; set; } = null!;

    public string MaPhuCap { get; set; } = null!;

    public int SoTienPhuCap { get; set; }

    public DateTime NgayApDung { get; set; }

    public virtual Luong MaLuongNavigation { get; set; } = null!;

    public virtual Phucap MaPhuCapNavigation { get; set; } = null!;
}
