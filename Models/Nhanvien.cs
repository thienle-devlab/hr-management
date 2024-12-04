using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace HR_Management.Models
{
    public partial class Nhanvien
    {
        [Required(ErrorMessage = "Mã nhân viên không được để trống")]
        [Display(Name = "Mã nhân viên")]
        public string MaNhanVien { get; set; } = null!;

        [Required(ErrorMessage = "Tên nhân viên không được để trống")]
        [Display(Name = "Tên nhân viên")]
        public string TenNhanVien { get; set; } = null!;

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [Display(Name = "Ngày sinh")]
        public DateOnly NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public bool GioiTinh { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? AnhDaiDien { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; } = null!;

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "CCCD không được để trống")]
        [Display(Name = "CCCD")]
        public string Cccd { get; set; } = null!;

        [Required(ErrorMessage = "Chức vụ không được để trống")]
        [Display(Name = "Chức vụ")]
        public string MaChucVu { get; set; } = null!;

        [NotMapped]
        [Display(Name = "Ảnh đại diện")]
        public IFormFile? AnhDaiDienFile { get; set; } = null!;

        // Navigation Properties
        public virtual ICollection<Baocao> Baocaos { get; set; } = new List<Baocao>();
        public virtual ICollection<Chamcong> Chamcongs { get; set; } = new List<Chamcong>();
        public virtual ICollection<Luong> Luongs { get; set; } = new List<Luong>();
        public virtual ICollection<Hopdong> HopDongs { get; set; } = new List<Hopdong>();
        public virtual ICollection<Luongphucap> Luongphucaps { get; set; } = new List<Luongphucap>();
        public virtual ICollection<Khautruluong> Khautruluongs { get; set; } = new List<Khautruluong>();
        public virtual ICollection<Phat> Phats { get; set; } = new List<Phat>();
        public virtual ICollection<Thuong> Thuongs { get; set; } = new List<Thuong>();

        [ForeignKey("MaChucVu")]
        public virtual Chucvu? MaChucVuNavigation { get; set; } = null!;
        
        public virtual ICollection<Nghiphep> Nghipheps { get; set; } = new List<Nghiphep>();
        public virtual ICollection<Phancong> Phancongs { get; set; } = new List<Phancong>();
        public virtual Taikhoan? Taikhoan { get; set; }
    }
}