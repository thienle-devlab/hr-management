using HR_Management.Class;
using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class KhauTruLuongController : Controller
    {
        private readonly HrmContext _context;

        public KhauTruLuongController(HrmContext context)
        {
            _context = context;
        }

        // Danh sách khấu trừ lương
        public async Task<IActionResult> ListKhauTruLuong()
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
                var allKTL = await _context.Khautruluongs.ToListAsync();
                ViewBag.IsAdmin = true;
                return View(allKTL);
            }

            // Nếu không phải admin, chỉ cho phép xem tài khoản thuộc cùng phòng ban
            var ktlInSamePhongBan = await _context.Khautruluongs
                .Include(ktl => ktl.MaNhanVienNavigation)
                .Where(ktl => ktl.MaNhanVienNavigation.MaChucVuNavigation.MaPhongBan == userPhongBan)
                .ToListAsync();

            ViewBag.IsAdmin = false;
            return View(ktlInSamePhongBan);

            //var khauTruLuong = await _context.Khautruluongs.ToListAsync();
            //return View(khauTruLuong);
        }

        // Tạo khấu trừ lương mới (GET)
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public IActionResult CreateKhauTruLuong()
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
            ViewBag.KhauTruList = new SelectList(_context.Khautrus, "MaKhauTru", "TenKhauTru");
            return View();
        }

        // Tạo khấu trừ lương mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateKhauTruLuong([Bind("MaKhauTruLuong,MaNhanVien,MaKhauTru,KhoanKhauTru,NgayApDung")] Khautruluong khautruluong)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(khautruluong.MaKhauTruLuong))
                    {
                        khautruluong.MaKhauTruLuong = Guid.NewGuid().ToString();
                    }

                    _context.Add(khautruluong);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListKhauTruLuong));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo khấu trừ lương: " + ex.Message);
                }
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", khautruluong.MaNhanVien);
            ViewBag.KhauTruList = new SelectList(_context.Khautrus, "MaKhauTru", "TenKhauTru", khautruluong.MaKhauTru);
            return View(khautruluong);
        }

        // Sửa khấu trừ lương (GET)
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public async Task<IActionResult> EditKhauTruLuong(string maKhauTruLuong)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(maKhauTruLuong))
            {
                return NotFound();
            }

            var khautruluong = await _context.Khautruluongs.FindAsync(maKhauTruLuong);
            if (khautruluong == null)
            {
                return NotFound();
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", khautruluong.MaNhanVien);
            ViewBag.KhauTruList = new SelectList(_context.Khautrus, "MaKhauTru", "TenKhauTru", khautruluong.MaKhauTru);
            return View(khautruluong);
        }

        // Sửa khấu trừ lương (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditKhauTruLuong(string maKhauTruLuong, [Bind("MaKhauTruLuong,MaNhanVien,MaKhauTru,KhoanKhauTru,NgayApDung")] Khautruluong khautruluong)
        {
            Console.WriteLine("Starting EditKhauTruLuong method...");

            if (maKhauTruLuong != khautruluong.MaKhauTruLuong)
            {
                Console.WriteLine($"ID mismatch: {maKhauTruLuong} != {khautruluong.MaKhauTruLuong}");
                return NotFound();
            }

            try
            {
                // Tắt validation cho các navigation properties nếu có
                ModelState.Remove("MaNhanVienNavigation");
                ModelState.Remove("MaKhauTruNavigation");

                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid.");

                    // Kiểm tra xem khấu trừ lương đã tồn tại chưa
                    var existingKhauTruLuong = await _context.Khautruluongs.FindAsync(khautruluong.MaKhauTruLuong);
                    if (existingKhauTruLuong == null)
                    {
                        Console.WriteLine("KhauTruLuong không tồn tại.");
                        return NotFound();
                    }

                    // Cập nhật các trường của existingKhauTruLuong từ khautruluong
                    existingKhauTruLuong.MaNhanVien = khautruluong.MaNhanVien;
                    existingKhauTruLuong.MaKhauTru = khautruluong.MaKhauTru;
                    existingKhauTruLuong.KhoanKhauTru = khautruluong.KhoanKhauTru;
                    existingKhauTruLuong.NgayApDung = khautruluong.NgayApDung;

                    // Đánh dấu entity là đã được thay đổi
                    _context.Update(existingKhauTruLuong);

                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("KhauTruLuong đã được cập nhật thành công.");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!KhauTruLuongExists(khautruluong.MaKhauTruLuong))
                        {
                            Console.WriteLine("KhauTruLuong không tồn tại khi lưu.");
                            return NotFound();
                        }
                        else
                        {
                            Console.WriteLine($"Concurrency exception: {ex.Message}");
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ghi lại chi tiết của ngoại lệ nội bộ
                        Console.WriteLine($"Exception: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        }
                        ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                    }

                    return RedirectToAction(nameof(ListKhauTruLuong));
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
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

            // Truyền danh sách lương và khấu trừ vào ViewBag
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", khautruluong.MaNhanVien);
            ViewBag.KhauTruList = new SelectList(_context.Khautrus, "MaKhauTru", "TenKhauTru", khautruluong.MaKhauTru);

            return View(khautruluong);
        }

        // Kiểm tra xem khấu trừ lương có tồn tại trong cơ sở dữ liệu hay không
        private bool KhauTruLuongExists(string maKhauTruLuong)
        {
            return _context.Khautruluongs.Any(e => e.MaKhauTruLuong == maKhauTruLuong);
        }


        // Xóa khấu trừ lương (GET)
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public async Task<IActionResult> DeleteKhauTruLuong(string maKhauTruLuong)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(maKhauTruLuong))
            {
                return NotFound();
            }

            var khautruluong = await _context.Khautruluongs
                .Include(k => k.MaNhanVienNavigation)
                .Include(k => k.MaKhauTruNavigation)
                .FirstOrDefaultAsync(m => m.MaKhauTruLuong == maKhauTruLuong);

            if (khautruluong == null)
            {
                return NotFound();
            }

            return View(khautruluong);
        }

        // Xóa khấu trừ lương (POST)
        [HttpPost, ActionName("DeleteKhauTruLuong")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteKhauTruLuongConfirmed(string maKhauTruLuong)
        {
            var khautruluong = await _context.Khautruluongs.FindAsync(maKhauTruLuong);
            if (khautruluong != null)
            {
                _context.Khautruluongs.Remove(khautruluong);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListKhauTruLuong));
        }

        // Kiểm tra khấu trừ lương tồn tại
        private bool KhautruluongExists(string maKhauTruLuong)
        {
            return _context.Khautruluongs.Any(e => e.MaKhauTruLuong == maKhauTruLuong);
        }
    }
}

