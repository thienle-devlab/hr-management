using HR_Management.Class;
using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class ThuongController : Controller
    {
        private readonly HrmContext _context;

        public ThuongController(HrmContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách thưởng
        public async Task<IActionResult> ListThuong()
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Lấy thông tin nhân viên từ DB để kiểm tra quyền
            var nhanVien = await _context.Nhanviens
                .Include(nv => nv.MaChucVuNavigation)
                .ThenInclude(cv => cv.MaPhongBanNavigation)
                .Where(nv => nv.MaNhanVien == maNhanVien)
                .Select(nv => new
                {
                    nv.MaChucVu,
                    nv.MaChucVuNavigation.MaPhongBan
                })
                .FirstOrDefaultAsync();

            if (nhanVien == null)
            {
                return RedirectToAction("Auth", "Login");
            }

            bool isAdmin = nhanVien.MaChucVu == "CV001" && nhanVien.MaPhongBan == "PB001";
            string userPhongBan = nhanVien.MaPhongBan;

            // Kiểm tra quyền admin: Nếu là admin (CV001, PB001) thì cho phép xem tất cả tài khoản
            if (isAdmin)
            {
                var allThuong = await _context.Thuongs.ToListAsync();
                ViewBag.IsAdmin = true;
                return View(allThuong);
            }

            // Nếu không phải admin, chỉ cho phép xem tài khoản thuộc cùng phòng ban
            var thuongInSamePhongBan = await _context.Thuongs
                .Include(thuong => thuong.MaNhanVienNavigation)
                .Where(thuong => thuong.MaNhanVienNavigation.MaChucVuNavigation.MaPhongBan == userPhongBan)
                .ToListAsync();

            ViewBag.IsAdmin = false;
            return View(thuongInSamePhongBan);

            //var thuong = await _context.Thuongs.ToListAsync();
            //return View(thuong);
        }

        // Hiển thị form thêm thưởng
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        [HttpGet]
        public IActionResult CreateThuong()
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
            return View();
        }

        // Xử lý form thêm thưởng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateThuong([Bind("MaThuong,MaNhanVien,NgayThuong,SoTienThuong,LyDoThuong")] Thuong thuong)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var nhanVien = await _context.Nhanviens.FindAsync(thuong.MaNhanVien);
                    if (nhanVien == null)
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên không tồn tại.");
                        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", thuong.MaNhanVien);
                        return View(thuong);
                    }

                    thuong.MaThuong = string.IsNullOrEmpty(thuong.MaThuong) ? Guid.NewGuid().ToString() : thuong.MaThuong;
                    _context.Thuongs.Add(thuong);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListThuong));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm thưởng.");
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", thuong.MaNhanVien);
            return View(thuong);
        }

        // Hiển thị form sửa thưởng
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        [HttpGet]
        public async Task<IActionResult> EditThuong(string maThuong)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(maThuong))
                return NotFound();

            var thuong = await _context.Thuongs.FirstOrDefaultAsync(t => t.MaThuong == maThuong);
            if (thuong == null)
                return NotFound();

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", thuong.MaNhanVien);
            return View(thuong);
        }

        // Xử lý form sửa thưởng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditThuong(string maThuong, [Bind("MaThuong,MaNhanVien,NgayThuong,SoTienThuong,LyDoThuong")] Thuong thuong)
        {
            if (maThuong != thuong.MaThuong)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingThuong = await _context.Thuongs.FindAsync(maThuong);
                    if (existingThuong == null)
                        return NotFound();

                    var nhanVien = await _context.Nhanviens.FindAsync(thuong.MaNhanVien);
                    if (nhanVien == null)
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên không tồn tại.");
                        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", thuong.MaNhanVien);
                        return View(thuong);
                    }

                    existingThuong.MaNhanVien = thuong.MaNhanVien;
                    existingThuong.NgayThuong = thuong.NgayThuong;
                    existingThuong.SoTienThuong = thuong.SoTienThuong;
                    existingThuong.LyDoThuong = thuong.LyDoThuong;

                    _context.Update(existingThuong);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListThuong));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi sửa thưởng.");
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", thuong.MaNhanVien);
            return View(thuong);
        }

        // Xóa thưởng (GET)
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        [HttpGet]
        public async Task<IActionResult> DeleteThuong(string maThuong)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(maThuong))
                return NotFound();

            var thuong = await _context.Thuongs
                .Include(t => t.MaNhanVienNavigation)
                .FirstOrDefaultAsync(t => t.MaThuong == maThuong);

            if (thuong == null)
                return NotFound();

            return View(thuong);
        }

        // Xóa thưởng (POST)
        [HttpPost, ActionName("DeleteThuong")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteThuongConfirmed(string maThuong)
        {
            var thuong = await _context.Thuongs.FindAsync(maThuong);
            if (thuong != null)
            {
                _context.Thuongs.Remove(thuong);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ListThuong));
        }
    }
}
