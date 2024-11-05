using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class LuongPhuCapController : Controller
    {
        private readonly HrmContext _context;

        // Constructor
        public LuongPhuCapController(HrmContext context)
        {
            _context = context;
        }

        // Danh sách lương phụ cấp
        public async Task<IActionResult> ListLuongPC()
        {
            var luongPC = await _context.Luongphucaps.ToListAsync();
            return View(luongPC);
        }

        // Thêm lương phụ cấp
        public IActionResult CreateLuongPC()
        {
            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong");
            ViewBag.PhuCapList = new SelectList(_context.Phucaps, "MaPhuCap", "TenPhuCap");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLuongPC([Bind("MaLuongPhuCap,MaLuong,MaPhuCap,SoTienPhuCap,NgayApDung")] Luongphucap luongPhuCap)
        {
            try
            {
                // Tắt validation cho MaLuongNavigation và MaPhuCapNavigation
                ModelState.Remove("MaLuongNavigation");
                ModelState.Remove("MaPhuCapNavigation");

                if (ModelState.IsValid)
                {
                    // Kiểm tra tồn tại của mã lương và mã phụ cấp
                    var maLuong = await _context.Luongs.FindAsync(luongPhuCap.MaLuong);
                    var maPhuCap = await _context.Phucaps.FindAsync(luongPhuCap.MaPhuCap);

                    if (maLuong == null)
                    {
                        ModelState.AddModelError("MaLuong", "Mã lương không tồn tại");
                    }

                    if (maPhuCap == null)
                    {
                        ModelState.AddModelError("MaPhuCap", "Mã phụ cấp không tồn tại");
                    }

                    if (ModelState.IsValid)
                    {
                        _context.Add(luongPhuCap);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(ListLuongPC));
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
            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "TenLuong");
            ViewBag.PhuCapList = new SelectList(_context.Phucaps, "MaPhuCap", "TenPhuCap");
            return View(luongPhuCap);
        }

        // Sửa lương phụ cấp
        [HttpGet]
        public async Task<IActionResult> EditLuongPC(string maLuongPhuCap)
        {
            Console.WriteLine("Starting EditLuongPC method...");

            if (string.IsNullOrEmpty(maLuongPhuCap))
            {
                Console.WriteLine("ID is null or empty");
                return NotFound();
            }

            var luongPhuCap = await _context.Luongphucaps.FirstOrDefaultAsync(m => m.MaLuongPhuCap == maLuongPhuCap);

            if (luongPhuCap == null)
            {
                Console.WriteLine("LuongPhuCap not found");
                return NotFound();
            }

            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", luongPhuCap.MaLuong);
            ViewBag.PhuCapList = new SelectList(_context.Phucaps, "MaPhuCap", "TenPhuCap", luongPhuCap.MaPhuCap);
            return View(luongPhuCap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLuongPC(string maLuongPhuCap, [Bind("MaLuongPhuCap,MaLuong,MaPhuCap,SoTienPhuCap,NgayApDung")] Luongphucap luongPhuCap)
        {
            Console.WriteLine("Starting EditLuongPC method...");

            if (maLuongPhuCap != luongPhuCap.MaLuongPhuCap)
            {
                Console.WriteLine($"ID mismatch: {maLuongPhuCap} != {luongPhuCap.MaLuongPhuCap}");
                return NotFound();
            }

            try
            {
                // Tắt validation cho MaLuongNavigation và MaPhuCapNavigation nếu không cần thiết
                ModelState.Remove("MaLuongNavigation");
                ModelState.Remove("MaPhuCapNavigation");

                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid.");

                    // Kiểm tra xem lương phụ cấp đã tồn tại chưa
                    var existingLuongPhuCap = await _context.Luongphucaps.FindAsync(luongPhuCap.MaLuongPhuCap);
                    if (existingLuongPhuCap == null)
                    {
                        Console.WriteLine("Lương phụ cấp không tồn tại.");
                        return NotFound();
                    }

                    // Cập nhật các trường của existingLuongPhuCap từ luongPhuCap
                    existingLuongPhuCap.MaLuong = luongPhuCap.MaLuong;
                    existingLuongPhuCap.MaPhuCap = luongPhuCap.MaPhuCap;
                    existingLuongPhuCap.SoTienPhuCap = luongPhuCap.SoTienPhuCap;
                    existingLuongPhuCap.NgayApDung = luongPhuCap.NgayApDung;

                    // Đánh dấu entity là đã được thay đổi
                    _context.Update(existingLuongPhuCap);

                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Lương phụ cấp đã được cập nhật thành công.");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!LuongPhuCapExists(luongPhuCap.MaLuongPhuCap))
                        {
                            Console.WriteLine("Lương phụ cấp không tồn tại khi lưu.");
                            return NotFound();
                        }
                        else
                        {
                            Console.WriteLine($"Concurrency exception: {ex.Message}");
                            throw;
                        }
                    }

                    return RedirectToAction(nameof(ListLuongPC)); // Điều hướng đến danh sách lương phụ cấp
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

            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", luongPhuCap.MaLuong);
            ViewBag.PhuCapList = new SelectList(_context.Phucaps, "MaPhuCap", "TenPhuCap", luongPhuCap.MaPhuCap);
            return View(luongPhuCap);
        }

        private bool LuongPhuCapExists(string maLuongPhuCap)
        {
            return _context.Luongphucaps.Any(e => e.MaLuongPhuCap == maLuongPhuCap);
        }

        // Xoá lương phụ cấp
        [HttpGet]
        public async Task<IActionResult> DeleteLuongPC(string maLuongPhuCap)
        {
            Console.WriteLine("Starting DeleteLuongPC method...");

            if (string.IsNullOrEmpty(maLuongPhuCap))
            {
                Console.WriteLine("MaLuongPhuCap is null or empty");
                return NotFound();
            }

            var luongPhuCap = await _context.Luongphucaps
                .FirstOrDefaultAsync(lp => lp.MaLuongPhuCap == maLuongPhuCap);

            if (luongPhuCap == null)
            {
                Console.WriteLine("LuongPhuCap not found");
                return NotFound();
            }

            return View(luongPhuCap);
        }

        // Xóa Lương Phụ Cấp
        [HttpPost, ActionName("DeleteLuongPC")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLuongPCConfirmed(string maLuongPhuCap)
        {
            Console.WriteLine("Starting DeleteLuongPCConfirmed method...");

            var luongPhuCap = await _context.Luongphucaps
                .FirstOrDefaultAsync(lp => lp.MaLuongPhuCap == maLuongPhuCap);

            if (luongPhuCap == null)
            {
                Console.WriteLine("LuongPhuCap not found");
                return NotFound();
            }

            // Xóa Lương Phụ Cấp
            _context.Luongphucaps.Remove(luongPhuCap);
            await _context.SaveChangesAsync();

            Console.WriteLine("LuongPhuCap has been deleted successfully.");
            return RedirectToAction(nameof(ListLuongPC));
        }


    }
}
