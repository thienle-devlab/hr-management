using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class ChucVuController : Controller
    {
        private readonly HrmContext _context;

        public ChucVuController(HrmContext context)
        {
            _context = context;
        }

        // Danh sách chức vụ
        public async Task<IActionResult> ListChucVu()
        {
            var chucVu = await _context.Chucvus.ToListAsync();
            return View(chucVu);
        }

        // Tạo chức vụ mới (GET)
        public IActionResult CreateChucVu()
        {
            // Truyền danh sách phòng ban vào ViewBag
            ViewBag.PhongBanList = new SelectList(_context.Phongbans, "MaPhongBan", "TenPhongBan");

            // Trả về form tạo chức vụ
            return View();
        }

        // Tạo chức vụ mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChucVu([Bind("MaChucVu,TenChucVu,MaPhongBan")] Chucvu chucVu)
        {
            // Kiểm tra model state hợp lệ
            if (ModelState.IsValid)
            {
                try
                {
                    // Tắt validation cho MaPhongBanNavigation
                    ModelState.Remove("MaPhongBanNavigation");

                    // Kiểm tra xem mã phòng ban có tồn tại không
                    var phongBan = await _context.Phongbans.FindAsync(chucVu.MaPhongBan);
                    if (phongBan == null)
                    {
                        ModelState.AddModelError("MaPhongBan", "Mã phòng ban không tồn tại.");
                        ViewBag.PhongBanList = new SelectList(_context.Phongbans, "MaPhongBan", "TenPhongBan", chucVu.MaPhongBan);
                        return View(chucVu);
                    }

                    // Gán MaPhongBanNavigation cho chức vụ
                    chucVu.MaPhongBanNavigation = phongBan;

                    // Tạo mã chức vụ nếu không có
                    if (string.IsNullOrEmpty(chucVu.MaChucVu))
                    {
                        chucVu.MaChucVu = Guid.NewGuid().ToString();
                    }

                    // Thêm chức vụ vào cơ sở dữ liệu
                    _context.Add(chucVu);
                    await _context.SaveChangesAsync();

                    // Điều hướng đến danh sách chức vụ
                    return RedirectToAction(nameof(ListChucVu)); // Đảm bảo action ListChucVu tồn tại
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu có
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo chức vụ.");
                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
            }

            // Nếu model không hợp lệ, trả lại form tạo chức vụ
            ViewBag.PhongBanList = new SelectList(_context.Phongbans, "MaPhongBan", "TenPhongBan", chucVu.MaPhongBan);
            return View(chucVu);
        }

        // Hiển thị form sửa chức vụ
        [HttpGet]
        public async Task<IActionResult> EditChucVu(string maChucVu)
        {
            Console.WriteLine("Starting EditChucVu method...");

            if (string.IsNullOrEmpty(maChucVu))
            {
                Console.WriteLine("ID is null or empty");
                return NotFound();
            }

            var chucVu = await _context.Chucvus.FirstOrDefaultAsync(m => m.MaChucVu == maChucVu);

            if (chucVu == null)
            {
                Console.WriteLine("ChucVu not found");
                return NotFound();
            }

            // Truyền danh sách phòng ban vào ViewBag
            ViewBag.PhongBanList = new SelectList(_context.Phongbans, "MaPhongBan", "TenPhongBan", chucVu.MaPhongBan);

            return View(chucVu); // Trả về view để người dùng có thể sửa chức vụ
        }

        // Xử lý form sửa chức vụ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChucVu(string maChucVu, [Bind("MaChucVu,TenChucVu,MaPhongBan")] Chucvu chucVu)
        {
            Console.WriteLine("Starting EditChucVu method...");

            if (maChucVu != chucVu.MaChucVu)
            {
                Console.WriteLine($"ID mismatch: {maChucVu} != {chucVu.MaChucVu}");
                return NotFound();
            }

            try
            {
                // Tắt validation cho các navigation properties nếu có
                ModelState.Remove("MaPhongBanNavigation");

                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid.");

                    // Kiểm tra xem chức vụ đã tồn tại chưa
                    var existingChucVu = await _context.Chucvus.FindAsync(chucVu.MaChucVu);
                    if (existingChucVu == null)
                    {
                        Console.WriteLine("ChucVu không tồn tại.");
                        return NotFound();
                    }

                    // Cập nhật các trường của existingChucVu từ chucVu
                    existingChucVu.TenChucVu = chucVu.TenChucVu;
                    existingChucVu.MaPhongBan = chucVu.MaPhongBan;

                    // Đánh dấu entity là đã được thay đổi
                    _context.Update(existingChucVu);

                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("ChucVu đã được cập nhật thành công.");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!ChucVuExists(chucVu.MaChucVu))
                        {
                            Console.WriteLine("ChucVu không tồn tại khi lưu.");
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

                    return RedirectToAction(nameof(ListChucVu));
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

            ViewBag.PhongBanList = new SelectList(_context.Phongbans, "MaPhongBan", "TenPhongBan", chucVu.MaPhongBan);
            return View(chucVu);
        }

        // Kiểm tra xem chức vụ có tồn tại trong cơ sở dữ liệu hay không
        private bool ChucVuExists(string maChucVu)
        {
            return _context.Chucvus.Any(e => e.MaChucVu == maChucVu);
        }

        // Xóa chức vụ (GET)
        public async Task<IActionResult> DeleteChucVu(string MaChucVu)
        {
            if (string.IsNullOrEmpty(MaChucVu))
            {
                return NotFound();
            }

            var chucVu = await _context.Chucvus
                .Include(c => c.MaPhongBanNavigation)
                .FirstOrDefaultAsync(c => c.MaChucVu == MaChucVu);

            if (chucVu == null)
            {
                return NotFound();
            }

            return View(chucVu);
        }

        // Xóa chức vụ (POST)
        [HttpPost, ActionName("DeleteChucVu")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChucVuConfirmed(string MaChucVu)
        {
            var chucVu = await _context.Chucvus.FindAsync(MaChucVu);
            if (chucVu != null)
            {
                _context.Chucvus.Remove(chucVu);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListChucVu));
        }
    }
}
