using System;
using System.Collections.Generic;

namespace HR_Management.Models
{
    public class Hopdong
    {
        public string MaHopDong { get; set; } = null!;

        public string MaNhanVien { get; set; } = null!;

        public DateTime NgayKy { get; set; } 

        public DateTime NgayBatDau { get; set; }

        public DateTime? NgayKetThuc { get; set; }

        public string LoaiHopDong { get; set; } = null!;

        public int MucLuong { get; set; }

        public string TrangThai { get; set; } = null!;

        public virtual Nhanvien MaNhanVienNavigation { get; set; } = null!;
    }
}
