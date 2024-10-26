using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Phongban
{
    public string MaPhongBan { get; set; } = null!;

    public string TenPhongBan { get; set; } = null!;

    public virtual ICollection<Chucvu> Chucvus { get; set; } = new List<Chucvu>();
}
