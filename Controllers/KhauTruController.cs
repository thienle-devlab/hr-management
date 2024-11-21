using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class KhauTruController : Controller
    {
        private readonly HrmContext _context;

        public KhauTruController(HrmContext context)
        {
            _context = context;
        }

        // Danh sách khấu trừ
        public async Task<IActionResult> ListKhauTru()
        {
            var khautrus = await _context.Khautrus.ToListAsync();
            return View(khautrus);
        }

        // Tạo mới khấu trừ (GET)
        public IActionResult CreateKhauTru()
        {
            return View();
        }

        // Tạo mới khấu trừ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateKhauTru([Bind("MaKhauTru,TenKhauTru")] Khautru khauTru)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khauTru);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListKhauTru));
            }
            return View(khauTru);
        }

        // Chỉnh sửa khấu trừ (GET)
        public async Task<IActionResult> EditKhauTru(string MaKhauTru)
        {
            if (string.IsNullOrEmpty(MaKhauTru))
            {
                return NotFound();
            }

            var khautru = await _context.Khautrus.FindAsync(MaKhauTru);
            if (khautru == null)
            {
                return NotFound();
            }

            return View(khautru);
        }

        // Chỉnh sửa khấu trừ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditKhauTru(string MaKhauTru, [Bind("MaKhauTru,TenKhauTru")] Khautru khautru)
        {
            if (MaKhauTru != khautru.MaKhauTru)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khautru);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListKhauTru));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhautruExists(khautru.MaKhauTru))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(khautru);
        }

        private bool KhautruExists(string MaKhauTru)
        {
            return _context.Khautrus.Any(e => e.MaKhauTru == MaKhauTru);
        }


        // Xóa khấu trừ (GET)
        public async Task<IActionResult> DeleteKhauTru(string MaKhauTru)
        {
            if (string.IsNullOrEmpty(MaKhauTru))
            {
                return NotFound();
            }

            var khautru = await _context.Khautrus.FindAsync(MaKhauTru);
            if (khautru == null)
            {
                return NotFound();
            }

            return View(khautru);
        }

        // Xóa khấu trừ (POST)
        [HttpPost, ActionName("DeleteKhauTru")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteKhauTruConfirmed(string MaKhauTru)
        {
            var khautru = await _context.Khautrus.FindAsync(MaKhauTru);
            if (khautru != null)
            {
                _context.Khautrus.Remove(khautru);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListKhauTru));
        }
    }
}
