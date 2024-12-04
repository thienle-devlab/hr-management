using HR_Management.Class;
using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class PhatController : Controller
    {
        private readonly HrmContext _context;

        public PhatController(HrmContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách phạt
        public async Task<IActionResult> ListPhat()
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
                var allPhat = await _context.Phats.ToListAsync();
                ViewBag.IsAdmin = true;
                return View(allPhat);
            }

            // Nếu không phải admin, chỉ cho phép xem tài khoản thuộc cùng phòng ban
            var phatInSamePhongBan = await _context.Phats
                .Include(phat => phat.MaNhanVienNavigation)
                .Where(phat => phat.MaNhanVienNavigation.MaChucVuNavigation.MaPhongBan == userPhongBan)
                .ToListAsync();

            ViewBag.IsAdmin = false;
            return View(phatInSamePhongBan);


            //var phat = await _context.Phats.ToListAsync();
            //return View(phat);
        }

        // Hiển thị form thêm phạt
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        [HttpGet]
        public IActionResult CreatePhat()
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");

            }
            // Truyền danh sách lương vào ViewBag
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");

            // Trả về form thêm phạt
            return View();
        }

        // Xử lý form thêm phạt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePhat([Bind("MaPhat,MaNhanVien,NgayPhat,SoTienPhat,LyDoPhat")] Phat phat)
        {
            // Kiểm tra model state hợp lệ
            if (ModelState.IsValid)
            {
                try
                {
                    // Tắt validation cho MaNhanVienNavigation
                    ModelState.Remove("MaNhanVienNavigation");

                    // Kiểm tra xem mã lương có tồn tại không
                    var nhanVien = await _context.Nhanviens.FindAsync(phat.MaNhanVien);
                    if (nhanVien == null)
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân không tồn tại.");
                        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", phat.MaNhanVien);
                        return View(phat);
                    }

                    // Gán MaNhanVienNavigation cho phạt
                    phat.MaNhanVienNavigation = nhanVien;

                    // Tạo mã phạt nếu không có
                    if (string.IsNullOrEmpty(phat.MaPhat))
                    {
                        phat.MaPhat = Guid.NewGuid().ToString();
                    }

                    // Thêm phạt vào cơ sở dữ liệu
                    _context.Add(phat);
                    await _context.SaveChangesAsync();

                    // Điều hướng đến danh sách phạt
                    return RedirectToAction(nameof(ListPhat));
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu có
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm phạt.");
                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
            }

            // Nếu model không hợp lệ, trả lại form thêm phạt
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", phat.MaNhanVien);
            return View(phat);
        }

        // Hiển thị form sửa phạt
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        [HttpGet]
        public async Task<IActionResult> EditPhat(string maPhat)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(maPhat))
            {
                return NotFound();
            }

            // Tìm phạt theo mã
            var phat = await _context.Phats.FirstOrDefaultAsync(p => p.MaPhat == maPhat);
            if (phat == null)
            {
                return NotFound();
            }

            // Truyền danh sách lương vào ViewBag
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", phat.MaNhanVien);

            return View(phat); // Trả về view để người dùng có thể sửa phạt
        }

        // Xử lý form sửa phạt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhat(string maPhat, [Bind("MaPhat,MaNhanVien,NgayPhat,SoTienPhat,LyDoPhat")] Phat phat)
        {
            if (maPhat != phat.MaPhat)
            {
                return NotFound();
            }

            try
            {
                // Kiểm tra ModelState hợp lệ
                if (ModelState.IsValid)
                {
                    // Kiểm tra phạt đã tồn tại trong DB chưa
                    var existingPhat = await _context.Phats.FindAsync(phat.MaPhat);
                    if (existingPhat == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra mã nv có tồn tại không
                    var nhanVien = await _context.Nhanviens.FindAsync(phat.MaNhanVien);
                    if (nhanVien == null)
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên không tồn tại.");
                        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", phat.MaNhanVien);
                        return View(phat);
                    }

                    // Cập nhật các trường từ phat sang existingPhat
                    existingPhat.MaNhanVien = phat.MaNhanVien;
                    existingPhat.NgayPhat = phat.NgayPhat;
                    existingPhat.SoTienPhat = phat.SoTienPhat;
                    existingPhat.LyDoPhat = phat.LyDoPhat;

                    // Cập nhật vào DB
                    _context.Update(existingPhat);
                    await _context.SaveChangesAsync();

                    // Điều hướng đến danh sách phạt
                    return RedirectToAction(nameof(ListPhat));
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                ModelState.AddModelError("", "Có lỗi xảy ra khi sửa phạt.");
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            // Nếu có lỗi, trả lại form sửa phạt
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", phat.MaNhanVien);
            return View(phat);
        }

        // Phương thức kiểm tra phạt có tồn tại không
        private bool PhatExists(string maPhat)
        {
            return _context.Phats.Any(e => e.MaPhat == maPhat);
        }

        // Xóa phạt (GET)
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public async Task<IActionResult> DeletePhat(string MaPhat)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(MaPhat))
            {
                return NotFound();
            }

            var phat = await _context.Phats
                .Include(p => p.MaNhanVienNavigation)
                .FirstOrDefaultAsync(p => p.MaPhat == MaPhat);

            if (phat == null)
            {
                return NotFound();
            }

            return View(phat);
        }

        // Xóa phạt (POST)
        [HttpPost, ActionName("DeletePhat")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhatConfirmed(string MaPhat)
        {
            var phat = await _context.Phats.FindAsync(MaPhat);
            if (phat != null)
            {
                _context.Phats.Remove(phat);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListPhat));
        }

    }
}
