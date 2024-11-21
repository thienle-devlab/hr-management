using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class ThuongController : Controller
    {
        private readonly HrmContext _context;

        public ThuongController(HrmContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách thưởng
        public async Task<IActionResult> ListThuong()
        {
            var thuong = await _context.Thuongs.ToListAsync();
            return View(thuong);
        }

        // Hiển thị form thêm thưởng
        [HttpGet]
        public IActionResult CreateThuong()
        {
            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong");
            return View();
        }

        // Xử lý form thêm thưởng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateThuong([Bind("MaThuong,MaLuong,NgayThuong,SoTienThuong,LyDoThuong")] Thuong thuong)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var luong = await _context.Luongs.FindAsync(thuong.MaLuong);
                    if (luong == null)
                    {
                        ModelState.AddModelError("MaLuong", "Mã lương không tồn tại.");
                        ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", thuong.MaLuong);
                        return View(thuong);
                    }

                    thuong.MaThuong = string.IsNullOrEmpty(thuong.MaThuong) ? Guid.NewGuid().ToString() : thuong.MaThuong;
                    _context.Thuongs.Add(thuong);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListThuong));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi thêm thưởng.");
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", thuong.MaLuong);
            return View(thuong);
        }

        // Hiển thị form sửa thưởng
        [HttpGet]
        public async Task<IActionResult> EditThuong(string maThuong)
        {
            if (string.IsNullOrEmpty(maThuong))
                return NotFound();

            var thuong = await _context.Thuongs.FirstOrDefaultAsync(t => t.MaThuong == maThuong);
            if (thuong == null)
                return NotFound();

            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", thuong.MaLuong);
            return View(thuong);
        }

        // Xử lý form sửa thưởng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditThuong(string maThuong, [Bind("MaThuong,MaLuong,NgayThuong,SoTienThuong,LyDoThuong")] Thuong thuong)
        {
            if (maThuong != thuong.MaThuong)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingThuong = await _context.Thuongs.FindAsync(maThuong);
                    if (existingThuong == null)
                        return NotFound();

                    var luong = await _context.Luongs.FindAsync(thuong.MaLuong);
                    if (luong == null)
                    {
                        ModelState.AddModelError("MaLuong", "Mã lương không tồn tại.");
                        ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", thuong.MaLuong);
                        return View(thuong);
                    }

                    existingThuong.MaLuong = thuong.MaLuong;
                    existingThuong.NgayThuong = thuong.NgayThuong;
                    existingThuong.SoTienThuong = thuong.SoTienThuong;
                    existingThuong.LyDoThuong = thuong.LyDoThuong;

                    _context.Update(existingThuong);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListThuong));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi sửa thưởng.");
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            ViewBag.LuongList = new SelectList(_context.Luongs, "MaLuong", "MaLuong", thuong.MaLuong);
            return View(thuong);
        }

        // Xóa thưởng (GET)
        [HttpGet]
        public async Task<IActionResult> DeleteThuong(string maThuong)
        {
            if (string.IsNullOrEmpty(maThuong))
                return NotFound();

            var thuong = await _context.Thuongs
                .Include(t => t.MaLuongNavigation)
                .FirstOrDefaultAsync(t => t.MaThuong == maThuong);

            if (thuong == null)
                return NotFound();

            return View(thuong);
        }

        // Xóa thưởng (POST)
        [HttpPost, ActionName("DeleteThuong")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteThuongConfirmed(string maThuong)
        {
            var thuong = await _context.Thuongs.FindAsync(maThuong);
            if (thuong != null)
            {
                _context.Thuongs.Remove(thuong);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ListThuong));
        }
    }
}
