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
            var phuCap = await _context.Phucaps.ToListAsync();
            return View(phuCap);
        }

        // Thêm phụ cấp
        public IActionResult CreatePhuCap()
        {
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
        public async Task<IActionResult> EditPhuCap(string maPhuCap)
        {
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
        public async Task<IActionResult> DeletePhuCap(string maPhuCap)
        {
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
