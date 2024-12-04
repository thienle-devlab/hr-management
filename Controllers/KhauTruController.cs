using HR_Management.Class;
using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class KhauTruController : Controller
    {
        private readonly HrmContext _context;

        public KhauTruController(HrmContext context)
        {
            _context = context;
        }

        // Danh sách khấu trừ
        public async Task<IActionResult> ListKhauTru()
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

            ViewBag.IsAdmin = isAdmin;

            var khautrus = await _context.Khautrus.ToListAsync();
            return View(khautrus);
        }

        // Tạo mới khấu trừ (GET)
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public IActionResult CreateKhauTru()
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // Tạo mới khấu trừ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateKhauTru([Bind("MaKhauTru,TenKhauTru")] Khautru khauTru)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khauTru);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListKhauTru));
            }
            return View(khauTru);
        }

        // Chỉnh sửa khấu trừ (GET)
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public async Task<IActionResult> EditKhauTru(string MaKhauTru)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(MaKhauTru))
            {
                return NotFound();
            }

            var khautru = await _context.Khautrus.FindAsync(MaKhauTru);
            if (khautru == null)
            {
                return NotFound();
            }

            return View(khautru);
        }

        // Chỉnh sửa khấu trừ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditKhauTru(string MaKhauTru, [Bind("MaKhauTru,TenKhauTru")] Khautru khautru)
        {
            if (MaKhauTru != khautru.MaKhauTru)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khautru);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListKhauTru));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhautruExists(khautru.MaKhauTru))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(khautru);
        }

        private bool KhautruExists(string MaKhauTru)
        {
            return _context.Khautrus.Any(e => e.MaKhauTru == MaKhauTru);
        }


        // Xóa khấu trừ (GET)
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public async Task<IActionResult> DeleteKhauTru(string MaKhauTru)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(MaKhauTru))
            {
                return NotFound();
            }

            var khautru = await _context.Khautrus.FindAsync(MaKhauTru);
            if (khautru == null)
            {
                return NotFound();
            }

            return View(khautru);
        }

        // Xóa khấu trừ (POST)
        [HttpPost, ActionName("DeleteKhauTru")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteKhauTruConfirmed(string MaKhauTru)
        {
            var khautru = await _context.Khautrus.FindAsync(MaKhauTru);
            if (khautru != null)
            {
                _context.Khautrus.Remove(khautru);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListKhauTru));
        }
    }
}
