using Microsoft.AspNetCore.Mvc;
using HR_Management.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HR_Management.Controllers
{
    public class LuongController : Controller
    {
        private readonly HrmContext _context;

        // Constructor
        public LuongController(HrmContext context)
        {
            _context = context;
        }

        // Danh sách lương
        public async Task<IActionResult> ListLuong()
        {
            var luong = await _context.Luongs.ToListAsync();
            return View(luong);
        }

        // Thêm Lương
        public IActionResult CreateLuong()
        {
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
            return View(new Luong { NgayNhanLuong = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLuong([Bind("MaLuong,MaNhanVien,LuongCoBan,NgayNhanLuong,TrangThai")] Luong luong)
        {
            try
            {
                // Tắt validation cho MaNhanVienNavigation
                ModelState.Remove("MaNhanVienNavigation");

                if (ModelState.IsValid)
                {
                    var nhanVien = await _context.Nhanviens.FindAsync(luong.MaNhanVien);
                    if (nhanVien == null)
                    {
                        ModelState.AddModelError("MaNhanVien", "Nhân viên không tồn tại");
                        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
                        return View(luong);
                    }

                    _context.Add(luong);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListLuong));
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

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
            return View(luong);
        }

        // Cập nhật lương
        [HttpGet]
        public async Task<IActionResult> EditLuong(string MaLuong)
        {
            Console.WriteLine("Starting EditLuong method...");

            if (string.IsNullOrEmpty(MaLuong))
            {
                Console.WriteLine("ID is null or empty");
                return NotFound();
            }

            var luong = await _context.Luongs
                .Include(l => l.MaNhanVienNavigation)
                .FirstOrDefaultAsync(m => m.MaLuong == MaLuong);

            if (luong == null)
            {
                Console.WriteLine("Luong not found");
                return NotFound();
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", luong.MaNhanVien);
            return View(luong);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLuong(string MaLuong, [Bind("MaLuong,MaNhanVien,LuongCoBan,NgayNhanLuong,TrangThai")] Luong luong)
        {
            Console.WriteLine("Starting EditLuong method...");

            if (MaLuong != luong.MaLuong)
            {
                Console.WriteLine($"ID mismatch: {MaLuong} != {luong.MaLuong}");
                return NotFound();
            }

            try
            {
                // Tắt validation cho MaNhanVienNavigation
                ModelState.Remove("MaNhanVienNavigation");

                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid.");

                    var nhanVien = await _context.Nhanviens.FindAsync(luong.MaNhanVien);
                    if (nhanVien == null)
                    {
                        Console.WriteLine("Nhân viên không tồn tại.");
                        ModelState.AddModelError("MaNhanVien", "Nhân viên không tồn tại");
                        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", luong.MaNhanVien);
                        return View(luong);
                    }

                    // Kiểm tra xem Luong đã tồn tại chưa
                    var existingLuong = await _context.Luongs.FindAsync(luong.MaLuong);
                    if (existingLuong == null)
                    {
                        Console.WriteLine("Lương không tồn tại.");
                        return NotFound();
                    }

                    // Cập nhật các trường của existingLuong từ luong
                    existingLuong.MaNhanVien = luong.MaNhanVien;
                    existingLuong.LuongCoBan = luong.LuongCoBan;
                    existingLuong.NgayNhanLuong = luong.NgayNhanLuong;
                    existingLuong.TrangThai = luong.TrangThai;

                    // Đánh dấu entity là đã được thay đổi
                    _context.Update(existingLuong);

                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Lương đã được cập nhật thành công.");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!LuongExists(luong.MaLuong))
                        {
                            Console.WriteLine("Lương không tồn tại khi lưu.");
                            return NotFound();
                        }
                        else
                        {
                            Console.WriteLine($"Concurrency exception: {ex.Message}");
                            throw;
                        }
                    }

                    return RedirectToAction(nameof(ListLuong));
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

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", luong.MaNhanVien);
            return View(luong);
        }

        private bool LuongExists(string MaLuong)
        {
            return _context.Luongs.Any(e => e.MaLuong == MaLuong);
        }

        // Xóa lương
        [HttpGet]
        public async Task<IActionResult> DeleteLuong(string MaLuong)
        {
            if (string.IsNullOrEmpty(MaLuong))
            {
                return NotFound();
            }

            var luong = await _context.Luongs
                .Include(l => l.MaNhanVienNavigation)
                .FirstOrDefaultAsync(m => m.MaLuong == MaLuong);

            if (luong == null)
            {
                return NotFound();
            }

            return View(luong);
        }

        [HttpPost, ActionName("DeleteLuong")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLuongConfirmed(string MaLuong)
        {
            try
            {
                var luong = await _context.Luongs.FindAsync(MaLuong);
                if (luong == null)
                {
                    return NotFound();
                }

                _context.Luongs.Remove(luong);
                await _context.SaveChangesAsync();

                // Chuyển hướng về trang danh sách sau khi xóa thành công
                return RedirectToAction(nameof(ListLuong));
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Error deleting Luong: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi xóa lương. Vui lòng thử lại.");

                // Trong trường hợp lỗi, load lại dữ liệu và trở về view
                var luong = await _context.Luongs
                    .Include(l => l.MaNhanVienNavigation)
                    .FirstOrDefaultAsync(m => m.MaLuong == MaLuong);
                return View(luong);
            }
        }


    }
}
