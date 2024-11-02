using System;
using System.Collections.Generic;

namespace HR_Management.Models;

public partial class Taikhoan
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public string? MaNhanVien { get; set; } = null!;

    public virtual Nhanvien? MaNhanVienNavigation { get; set; } = null!;
}
