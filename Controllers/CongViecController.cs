using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HR_Management.Controllers
{
    public class CongViecController : Controller
    {
        private readonly HrmContext _context;

        // Constructor
        public CongViecController(HrmContext context)
        {
            _context = context;
        }

        // Lấy danh sách Công việc
        public async Task<IActionResult> ListCongViec()
        {
            var cv = await _context.Congviecs.ToListAsync();
            return View(cv);
        }

        // Thêm công việc
        public IActionResult CreateCV()
        {
            return View(new Congviec());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCV(Congviec congViec)
        {
            try
            {
                // Log dữ liệu trước khi insert
                Console.WriteLine($"MaCongViec: {congViec.MaCongViec}");
                Console.WriteLine($"TenCongViec: {congViec.TenCongViec}");
                Console.WriteLine($"MoTaCongViec: {congViec.MoTaCongViec}");

                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                    return View(congViec);
                }

                // Thêm công việc vào cơ sở dữ liệu
                _context.Congviecs.Add(congViec);
                await _context.SaveChangesAsync();
                Console.WriteLine("Công việc đã được lưu vào cơ sở dữ liệu");

                return RedirectToAction(nameof(ListCongViec));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                ModelState.AddModelError("", "Lỗi khi lưu vào database: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            return View(congViec);
        }

        // Cập nhật công việc
        public async Task<IActionResult> EditCV(string maCongViec)
        {
            var congViec = await _context.Congviecs.FindAsync(maCongViec);
            if (congViec == null)
            {
                return NotFound();
            }
            return View(congViec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCV(Congviec congViec)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(congViec);
                }

                // Lấy công việc hiện tại từ cơ sở dữ liệu
                var existingCongViec = await _context.Congviecs.AsNoTracking().FirstOrDefaultAsync(cv => cv.MaCongViec == congViec.MaCongViec);
                if (existingCongViec == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin công việc
                _context.Congviecs.Update(congViec);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListCongViec));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                ModelState.AddModelError("", "Lỗi khi cập nhật vào database: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            return View(congViec);
        }


        // Xóa công việc
        public async Task<IActionResult> DeleteCV(string maCongViec)
        {
            var congViec = await _context.Congviecs.FindAsync(maCongViec);
            if (congViec == null)
            {
                return NotFound();
            }
            return View("DeleteCV", congViec);
        }

        [HttpPost, ActionName("DeleteCV")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCVConfirmed(string maCongViec)
        {
            var congViec = await _context.Congviecs.FindAsync(maCongViec);
            if (congViec == null)
            {
                return NotFound();
            }

            try
            {
                // Xóa công việc khỏi cơ sở dữ liệu
                _context.Congviecs.Remove(congViec);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListCongViec));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            return View(congViec);
        }

    }
}
