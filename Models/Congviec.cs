using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Congviec
{
    public string MaCongViec { get; set; } = null!;

    public string TenCongViec { get; set; } = null!;

    public string? MoTaCongViec { get; set; }

    public virtual ICollection<Phancong> Phancongs { get; set; } = new List<Phancong>();
}
