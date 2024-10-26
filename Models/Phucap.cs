using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Phucap
{
    public string MaPhuCap { get; set; } = null!;

    public string TenPhuCap { get; set; } = null!;

    public virtual ICollection<Luongphucap> Luongphucaps { get; set; } = new List<Luongphucap>();
}
