using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class PhongBanController : Controller
    {
        private readonly HrmContext _context;

        public PhongBanController(HrmContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách phòng ban
        //public async Task<IActionResult> ListPhongBan()
        //{
        //    var phongBans = await _context.Phongbans.ToListAsync();
        //    return View(phongBans);
        //}

        // Hiển thị danh sách phòng ban
        [HttpGet]
        public async Task<IActionResult> ListPhongBan()
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

            var phongBans = await _context.Phongbans.ToListAsync();
            return View(phongBans);
        }


        // Tạo phòng ban mới (GET)
        public async Task<IActionResult> CreatePhongBan()
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
                .FirstOrDefaultAsync();

            if (nhanVien == null || (nhanVien.MaChucVu != "CV001" && nhanVien.MaChucVuNavigation.MaPhongBan != "PB001"))
            {
                return RedirectToAction("Login", "Auth");
            }

            bool isAdmin = nhanVien.MaChucVu == "CV001" && nhanVien.MaChucVuNavigation.MaPhongBan == "PB001";
            ViewBag.IsAdmin = isAdmin;

            return View(new Phongban());
        }

        // Tạo phòng ban mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePhongBan([Bind("MaPhongBan, TenPhongBan")] Phongban phongBan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phongBan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListPhongBan));
            }
            return View(phongBan);
        }

        // Chỉnh sửa phòng ban (GET)
        public async Task<IActionResult> EditPhongBan(string MaPhongBan)
        {
            if (string.IsNullOrEmpty(MaPhongBan))
            {
                return NotFound();
            }

            var phongBan = await _context.Phongbans.FindAsync(MaPhongBan);
            if (phongBan == null)
            {
                return NotFound();
            }

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
                .FirstOrDefaultAsync();

            if (nhanVien == null || (nhanVien.MaChucVu != "CV001" && nhanVien.MaChucVuNavigation.MaPhongBan != "PB001"))
            {
                return RedirectToAction("Login", "Auth");
            }

            bool isAdmin = nhanVien.MaChucVu == "CV001" && nhanVien.MaChucVuNavigation.MaPhongBan == "PB001";
            ViewBag.IsAdmin = isAdmin;

            return View(phongBan);
        }

        // Chỉnh sửa phòng ban (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhongBan(string MaPhongBan, [Bind("MaPhongBan, TenPhongBan")] Phongban phongBan)
        {
            if (MaPhongBan != phongBan.MaPhongBan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phongBan);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListPhongBan));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongBanExists(phongBan.MaPhongBan))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(phongBan);
        }

        private bool PhongBanExists(string MaPhongBan)
        {
            return _context.Phongbans.Any(e => e.MaPhongBan == MaPhongBan);
        }

        // Xóa phòng ban (GET)
        public async Task<IActionResult> DeletePhongBan(string MaPhongBan)
        {
            if (string.IsNullOrEmpty(MaPhongBan))
            {
                return NotFound();
            }

            var phongBan = await _context.Phongbans.FindAsync(MaPhongBan);
            if (phongBan == null)
            {
                return NotFound();
            }

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
                .FirstOrDefaultAsync();

            if (nhanVien == null || (nhanVien.MaChucVu != "CV001" && nhanVien.MaChucVuNavigation.MaPhongBan != "PB001"))
            {
                return RedirectToAction("Login", "Auth");
            }

            bool isAdmin = nhanVien.MaChucVu == "CV001" && nhanVien.MaChucVuNavigation.MaPhongBan == "PB001";
            ViewBag.IsAdmin = isAdmin;

            return View(phongBan);
        }

        // Xóa phòng ban (POST)
        [HttpPost, ActionName("DeletePhongBan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhongBanConfirmed(string MaPhongBan)
        {
            var phongBan = await _context.Phongbans.FindAsync(MaPhongBan);
            if (phongBan != null)
            {
                _context.Phongbans.Remove(phongBan);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListPhongBan));
        }

    }
}
