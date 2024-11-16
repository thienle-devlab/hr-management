using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class NghiPhepController : Controller
    {
        private readonly HrmContext _context;

        // Constructor
        public NghiPhepController(HrmContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách nhân viên nghỉ phép
        public async Task<IActionResult> ListNghiPhep()
        {
            var nghiPhep = await _context.Nghipheps.ToListAsync();
            return View(nghiPhep);
        }

        // Thêm nghỉ phép
        public IActionResult CreateNghiPhep()
        {
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
            return View(new Nghiphep { NgayBatDau = DateTime.Now, NgayKetThuc = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNghiPhep([Bind("MaNghiPhep,MaNhanVien,NgayBatDau,NgayKetThuc,LyDo,TrangThai")] Nghiphep nghiPhep)
        {
            try
            {
                // Tắt validation cho MaNhanVienNavigation nếu có
                ModelState.Remove("MaNhanVienNavigation");

                if (ModelState.IsValid)
                {
                    var nhanVien = await _context.Nhanviens.FindAsync(nghiPhep.MaNhanVien);
                    if (nhanVien == null)
                    {
                        ModelState.AddModelError("MaNhanVien", "Nhân viên không tồn tại");
                        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
                        return View(nghiPhep);
                    }

                    _context.Add(nghiPhep);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListNghiPhep));
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
            return View(nghiPhep);
        }

        // Sửa nghỉ phép
        public async Task<IActionResult> EditNghiPhep(string maNghiPhep)
        {
            if (string.IsNullOrEmpty(maNghiPhep))
            {
                return NotFound();
            }

            var nghiPhep = await _context.Nghipheps
                .FirstOrDefaultAsync(m => m.MaNghiPhep == maNghiPhep);

            if (nghiPhep == null)
            {
                return NotFound();
            }

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", nghiPhep.MaNhanVien);
            return View(nghiPhep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNghiPhep(string maNghiPhep, [Bind("MaNghiPhep,MaNhanVien,NgayBatDau,NgayKetThuc,LyDo,TrangThai")] Nghiphep nghiPhep)
        {
            Console.WriteLine("Starting EditNghiPhep method...");

            if (maNghiPhep != nghiPhep.MaNghiPhep)
            {
                Console.WriteLine($"ID mismatch: {maNghiPhep} != {nghiPhep.MaNghiPhep}");
                return NotFound();
            }

            try
            {
                // Tắt validation cho MaNhanVienNavigation
                ModelState.Remove("MaNhanVienNavigation");

                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid.");

                    var nhanVien = await _context.Nhanviens.FindAsync(nghiPhep.MaNhanVien);
                    if (nhanVien == null)
                    {
                        Console.WriteLine("Nhân viên không tồn tại.");
                        ModelState.AddModelError("MaNhanVien", "Nhân viên không tồn tại");
                        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", nghiPhep.MaNhanVien);
                        return View(nghiPhep);
                    }

                    // Kiểm tra xem NghiPhep đã tồn tại chưa
                    var existingNghiPhep = await _context.Nghipheps.FindAsync(nghiPhep.MaNghiPhep);
                    if (existingNghiPhep == null)
                    {
                        Console.WriteLine("Nghỉ phép không tồn tại.");
                        return NotFound();
                    }

                    // Cập nhật các trường của existingNghiPhep từ nghiPhep
                    existingNghiPhep.MaNhanVien = nghiPhep.MaNhanVien;
                    existingNghiPhep.NgayBatDau = nghiPhep.NgayBatDau;
                    existingNghiPhep.NgayKetThuc = nghiPhep.NgayKetThuc;
                    existingNghiPhep.LyDo = nghiPhep.LyDo;
                    existingNghiPhep.TrangThai = nghiPhep.TrangThai;

                    // Đánh dấu entity là đã được thay đổi
                    _context.Update(existingNghiPhep);

                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Nghỉ phép đã được cập nhật thành công.");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!NghiPhepExists(nghiPhep.MaNghiPhep))
                        {
                            Console.WriteLine("Nghỉ phép không tồn tại khi lưu.");
                            return NotFound();
                        }
                        else
                        {
                            Console.WriteLine($"Concurrency exception: {ex.Message}");
                            throw;
                        }
                    }

                    return RedirectToAction(nameof(ListNghiPhep));
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

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", nghiPhep.MaNhanVien);
            return View(nghiPhep);
        }

        private bool NghiPhepExists(string maNghiPhep)
        {
            return _context.Nghipheps.Any(e => e.MaNghiPhep == maNghiPhep);
        }

        // Xóa nghỉ phép
        [HttpGet]
        public async Task<IActionResult> DeleteNghiPhep(string maNghiPhep)
        {
            Console.WriteLine("Starting DeleteNghiPhep method...");

            if (string.IsNullOrEmpty(maNghiPhep))
            {
                Console.WriteLine("MaNghiPhep is null or empty");
                return NotFound();
            }

            var nghiPhep = await _context.Nghipheps
                .Include(n => n.MaNhanVienNavigation)
                .FirstOrDefaultAsync(m => m.MaNghiPhep == maNghiPhep);

            if (nghiPhep == null)
            {
                Console.WriteLine("NghiPhep not found");
                return NotFound();
            }

            return View(nghiPhep);
        }

        [HttpPost, ActionName("DeleteNghiPhep")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNghiPhepConfirmed(string maNghiPhep)
        {
            Console.WriteLine("Starting DeleteConfirmed method...");

            var nghiPhep = await _context.Nghipheps.FindAsync(maNghiPhep);
            if (nghiPhep == null)
            {
                Console.WriteLine("NghiPhep not found");
                return NotFound();
            }

            try
            {
                _context.Nghipheps.Remove(nghiPhep);
                await _context.SaveChangesAsync();
                Console.WriteLine("NghiPhep deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(nghiPhep);
            }

            return RedirectToAction(nameof(ListNghiPhep));
        }


    }
}
