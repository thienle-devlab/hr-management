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
        public async Task<IActionResult> ListLuong()
        {
            var luong = await _context.Luongs.ToListAsync();
            return View(luong);
        }

        // Thêm Lương
        public IActionResult ThemLuong()
        {
            ViewBag.NhanVienList = _context.Nhanviens.Select(e => new SelectListItem
            {
                Value = e.MaNhanVien,
                Text = e.TenNhanVien
            }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLuong(Luong luong)
        {
            try
            {

            }
            catch ()
            {

            }
        }
    }
}
