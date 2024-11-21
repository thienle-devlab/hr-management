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
            var phat = await _context.Phats.ToListAsync();
            return View(phat);
        }

        // Hiển thị form thêm phạt
        [HttpGet]
        public IActionResult CreatePhat()
        {
            // Truyền danh sách lương vào ViewBag
            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong");

            // Trả về form thêm phạt
            return View();
        }

        // Xử lý form thêm phạt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePhat([Bind("MaPhat,MaLuong,NgayPhat,SoTienPhat,LyDoPhat")] Phat phat)
        {
            // Kiểm tra model state hợp lệ
            if (ModelState.IsValid)
            {
                try
                {
                    // Tắt validation cho MaLuongNavigation
                    ModelState.Remove("MaLuongNavigation");

                    // Kiểm tra xem mã lương có tồn tại không
                    var luong = await _context.Luongs.FindAsync(phat.MaLuong);
                    if (luong == null)
                    {
                        ModelState.AddModelError("MaLuong", "Mã lương không tồn tại.");
                        ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", phat.MaLuong);
                        return View(phat);
                    }

                    // Gán MaLuongNavigation cho phạt
                    phat.MaLuongNavigation = luong;

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
            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", phat.MaLuong);
            return View(phat);
        }

        // Hiển thị form sửa phạt
        [HttpGet]
        public async Task<IActionResult> EditPhat(string maPhat)
        {
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
            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", phat.MaLuong);

            return View(phat); // Trả về view để người dùng có thể sửa phạt
        }

        // Xử lý form sửa phạt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhat(string maPhat, [Bind("MaPhat,MaLuong,NgayPhat,SoTienPhat,LyDoPhat")] Phat phat)
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

                    // Kiểm tra mã lương có tồn tại không
                    var luong = await _context.Luongs.FindAsync(phat.MaLuong);
                    if (luong == null)
                    {
                        ModelState.AddModelError("MaLuong", "Mã lương không tồn tại.");
                        ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", phat.MaLuong);
                        return View(phat);
                    }

                    // Cập nhật các trường từ phat sang existingPhat
                    existingPhat.MaLuong = phat.MaLuong;
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
            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", phat.MaLuong);
            return View(phat);
        }

        // Phương thức kiểm tra phạt có tồn tại không
        private bool PhatExists(string maPhat)
        {
            return _context.Phats.Any(e => e.MaPhat == maPhat);
        }

        // Xóa phạt (GET)
        public async Task<IActionResult> DeletePhat(string MaPhat)
        {
            if (string.IsNullOrEmpty(MaPhat))
            {
                return NotFound();
            }

            var phat = await _context.Phats
                .Include(p => p.MaLuongNavigation)
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
