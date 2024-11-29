using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Class
{
    public class CustomAuth : TypeFilterAttribute
    {
        public CustomAuth(string[] maChucVu = null, string[] maPhongBan = null) : base(typeof(CustomAuthorizationFilter))
        {
            Arguments = new object[] { maChucVu, maPhongBan };
        }
    }

    public class CustomAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly string[] _maChucVu;
        private readonly string[] _maPhongBan;
        private readonly HrmContext _context;

        public CustomAuthorizationFilter(string[] maChucVu, string[] maPhongBan, HrmContext context)
        {
            _maChucVu = maChucVu;
            _maPhongBan = maPhongBan;
            _context = context;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Lấy thông tin nhân viên từ Session khi đăng nhập
            var maNhanVien = context.HttpContext.Session.GetString("MaNhanVien");

            if(string.IsNullOrEmpty(maNhanVien))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }

            // Kiểm tra quyền truy cập
            var hasPermission = await CheckPermissionAsync(maNhanVien);

            if(!hasPermission)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }

        private async Task<bool> CheckPermissionAsync(string maNhanVien)
        {
            // Truy vấn lấy thông tin chức vụ và phòng ban của nhân viên
            var nhanVien = _context.Nhanviens
                .Include(nv => nv.MaChucVuNavigation)
                .ThenInclude(cv => cv.MaPhongBanNavigation)
                .Where(nv => nv.MaNhanVien == maNhanVien)
                .Select(nv => new
                {
                    MaChucVu = nv.MaChucVu,
                    MaPhongBan = nv.MaChucVuNavigation.MaPhongBan
                })
                .FirstOrDefault();

            if (nhanVien == null)
                return false;

            // Nếu không yêu cầu chức vụ và phòng ban cụ thể
            if ((_maChucVu == null || _maChucVu.Length == 0) && (_maPhongBan == null || _maPhongBan.Length == 0))
                return true;

            // Kiểm tra chức vụ nếu có yêu cầu
            if (_maChucVu != null && _maChucVu.Length > 0 && !_maChucVu.Contains(nhanVien.MaChucVu))
                return false;

            // Kiểm tra phòng ban nếu có yêu cầu
            if (_maPhongBan != null && _maPhongBan.Length > 0 && !_maPhongBan.Contains(nhanVien.MaPhongBan))
                return false;

            return true;
        }
    }
}
