using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Khautru
{
    public string MaKhauTru { get; set; } = null!;

    public string TenKhauTru { get; set; } = null!;

    public virtual ICollection<Khautruluong> Khautruluongs { get; set; } = new List<Khautruluong>();
}
