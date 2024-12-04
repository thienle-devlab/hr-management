using HR_Management.Class;
using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class PhuCapController : Controller
    {
        private readonly HrmContext _context;

        // Constructor
        public PhuCapController(HrmContext context)
        {
            _context = context;
        }

        // Danh sách phụ cấp
        public async Task<IActionResult> ListPhuCap()
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

            var phuCap = await _context.Phucaps.ToListAsync();
            return View(phuCap);
        }

        // Thêm phụ cấp
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public IActionResult CreatePhuCap()
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaPhuCap,TenPhuCap")] Phucap phuCap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phuCap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListPhuCap));
            }
            return View(phuCap);
        }

        // Sửa phụ cấp
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public async Task<IActionResult> EditPhuCap(string maPhuCap)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (maPhuCap == null)
            {
                return NotFound();
            }

            var phuCap = await _context.Phucaps.FindAsync(maPhuCap);
            if (phuCap == null)
            {
                return NotFound();
            }
            return View(phuCap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhuCap(string maPhuCap, [Bind("MaPhuCap,TenPhuCap")] Phucap phuCap)
        {
            if (maPhuCap != phuCap.MaPhuCap)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phuCap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhuCapExists(phuCap.MaPhuCap))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListPhuCap));
            }
            return View(phuCap);
        }

        private bool PhuCapExists(string maPhuCap)
        {
            return _context.Phucaps.Any(e => e.MaPhuCap == maPhuCap);
        }

        // Xóa phụ cấp
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public async Task<IActionResult> DeletePhuCap(string maPhuCap)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (maPhuCap == null)
            {
                return NotFound();
            }

            var phuCap = await _context.Phucaps
                .FirstOrDefaultAsync(m => m.MaPhuCap == maPhuCap);
            if (phuCap == null)
            {
                return NotFound();
            }

            return View(phuCap);
        }

        [HttpPost, ActionName("DeletePhuCap")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string maPhuCap)
        {
            var phuCap = await _context.Phucaps.FindAsync(maPhuCap);
            if (phuCap != null)
            {
                _context.Phucaps.Remove(phuCap);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListPhuCap));
        }

    }
}
