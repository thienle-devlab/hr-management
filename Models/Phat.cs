using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Phat
{
    public string MaPhat { get; set; } = null!;

    public string MaLuong { get; set; } = null!;

    public DateTime NgayPhat { get; set; }

    public int SoTienPhat { get; set; }

    public string? LyDoPhat { get; set; }

    public virtual Luong MaLuongNavigation { get; set; } = null!;
}
