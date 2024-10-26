using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Baocao
{
    public string MaBaoCao { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public string TenBaoCao { get; set; } = null!;

    public string DuLieuBaoCao { get; set; } = null!;

    public DateTime NgayTaoBaoCao { get; set; }

    public string LoaiBaoCao { get; set; } = null!;

    public string TrangThaiBaoCao { get; set; } = null!;

    public virtual Nhanvien MaNhanVienNavigation { get; set; } = null!;
}
