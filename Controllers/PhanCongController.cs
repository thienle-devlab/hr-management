using HR_Management.Class;
using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class PhanCongController : Controller
    {
        private readonly HrmContext _context;

        public PhanCongController(HrmContext context)
        {
            _context = context;
        }

        // List Phân công
        public async Task<IActionResult> ListPC()
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
                var allPC = await _context.Phancongs.ToListAsync();
                ViewBag.IsAdmin = true;
                return View(allPC);
            }

            // Nếu không phải admin, chỉ cho phép xem tài khoản thuộc cùng phòng ban
            var pcInSamePhongBan = await _context.Phancongs
                .Include(pc => pc.MaNhanVienNavigation)
                .Where(pc => pc.MaNhanVienNavigation.MaChucVuNavigation.MaPhongBan == userPhongBan)
                .ToListAsync();

            ViewBag.IsAdmin = false;
            return View(pcInSamePhongBan);

            //var phanCongs = _context.Phancongs.ToList();
            //return View(phanCongs);
        }

        // Create Phân công
        // Hiển thị form thêm phân công
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public IActionResult CreatePC()
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
            ViewBag.CongViecList = new SelectList(_context.Congviecs, "MaCongViec", "TenCongViec");
            return View();
        }

        // Xử lý form thêm phân công
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePC([Bind("MaPhanCong,MaNhanVien,MaCongViec,NgayBatDau,NgayKetThuc,TrangThai")] Phancong phanCong)
        {
            try
            {
                // Tắt validation cho các navigation properties nếu có
                ModelState.Remove("MaCongViecNavigation");
                ModelState.Remove("MaNhanVienNavigation");

                if (ModelState.IsValid)
                {
                    // Kiểm tra tồn tại của mã nhân viên và mã công việc
                    var nhanVien = await _context.Nhanviens.FindAsync(phanCong.MaNhanVien);
                    var congViec = await _context.Congviecs.FindAsync(phanCong.MaCongViec);

                    if (nhanVien == null)
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên không tồn tại");
                    }

                    if (congViec == null)
                    {
                        ModelState.AddModelError("MaCongViec", "Mã công việc không tồn tại");
                    }

                    if (ModelState.IsValid)
                    {
                        _context.Add(phanCong);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(ListPC));
                    }
                }

                // Log lỗi để debug
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Console.WriteLine($"Lỗi ModelState: {error.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            // Nếu có lỗi, truyền lại danh sách cho View
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
            ViewBag.CongViecList = new SelectList(_context.Congviecs, "MaCongViec", "TenCongViec");
            return View(phanCong);
        }

        // Hiển thị form sửa phân công
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        [HttpGet]
        public async Task<IActionResult> EditPC(string maPhanCong)
        {
            Console.WriteLine("Starting EditPC method...");

            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(maPhanCong))
            {
                Console.WriteLine("ID is null or empty");
                return NotFound();
            }

            var phanCong = await _context.Phancongs.FirstOrDefaultAsync(m => m.MaPhanCong == maPhanCong);

            if (phanCong == null)
            {
                Console.WriteLine("PhanCong not found");
                return NotFound();
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", phanCong.MaNhanVien);
            ViewBag.CongViecList = new SelectList(_context.Congviecs, "MaCongViec", "TenCongViec", phanCong.MaCongViec);
            return View(phanCong);
        }

        // Xử lý form sửa phân công
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPC(string maPhanCong, [Bind("MaPhanCong,MaNhanVien,MaCongViec,NgayBatDau,NgayKetThuc,TrangThai")] Phancong phanCong)
        {
            Console.WriteLine("Starting EditPC method...");

            if (maPhanCong != phanCong.MaPhanCong)
            {
                Console.WriteLine($"ID mismatch: {maPhanCong} != {phanCong.MaPhanCong}");
                return NotFound();
            }

            try
            {
                // Tắt validation cho các navigation properties nếu có
                ModelState.Remove("MaCongViecNavigation");
                ModelState.Remove("MaNhanVienNavigation");

                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid.");

                    // Kiểm tra xem phân công đã tồn tại chưa
                    var existingPhanCong = await _context.Phancongs.FindAsync(phanCong.MaPhanCong);
                    if (existingPhanCong == null)
                    {
                        Console.WriteLine("PhanCong không tồn tại.");
                        return NotFound();
                    }

                    // Cập nhật các trường của existingPhanCong từ phanCong
                    existingPhanCong.MaNhanVien = phanCong.MaNhanVien;
                    existingPhanCong.MaCongViec = phanCong.MaCongViec;
                    existingPhanCong.NgayBatDau = phanCong.NgayBatDau;
                    existingPhanCong.NgayKetThuc = phanCong.NgayKetThuc;
                    existingPhanCong.TrangThai = phanCong.TrangThai;

                    // Đánh dấu entity là đã được thay đổi
                    _context.Update(existingPhanCong);

                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("PhanCong đã được cập nhật thành công.");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!PhanCongExists(phanCong.MaPhanCong))
                        {
                            Console.WriteLine("PhanCong không tồn tại khi lưu.");
                            return NotFound();
                        }
                        else
                        {
                            Console.WriteLine($"Concurrency exception: {ex.Message}");
                            throw;
                        }
                    }

                    return RedirectToAction(nameof(ListPC)); // Điều hướng đến danh sách phân công
                }
                else
                {
                    Console.WriteLine("ModelState is not valid.");
                    // Log lỗi để debug
                    foreach (var modelStateEntry in ModelState.Values)
                    {
                        foreach (var error in modelStateEntry.Errors)
                        {
                            Console.WriteLine($"Lỗi ModelState: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", phanCong.MaNhanVien);
            ViewBag.CongViecList = new SelectList(_context.Congviecs, "MaCongViec", "TenCongViec", phanCong.MaCongViec);
            return View(phanCong);
        }

        private bool PhanCongExists(string maPhanCong)
        {
            return _context.Phancongs.Any(e => e.MaPhanCong == maPhanCong);
        }

        // Delete Phân công
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public IActionResult DeletePC(string maPhanCong)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (maPhanCong == null)
            {
                return NotFound();
            }

            var phanCong = _context.Phancongs
                .FirstOrDefault(m => m.MaPhanCong == maPhanCong);
            if (phanCong == null)
            {
                return NotFound();
            }

            return View(phanCong);
        }

        [HttpPost, ActionName("DeletePC")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string maPhanCong)
        {
            var phanCong = _context.Phancongs.Find(maPhanCong);
            if (phanCong != null)
            {
                _context.Phancongs.Remove(phanCong);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(ListPC));
        }

    }
}
