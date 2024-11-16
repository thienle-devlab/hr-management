using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HR_Management.Controllers
{
    public class ChamCongController : Controller
    {
        private readonly HrmContext _context;

        public ChamCongController(HrmContext context)
        {
            _context = context;
        }

        // GET: ChamCong
        public IActionResult ListChamCong()
        {
            var chamCongs = _context.Chamcongs.ToList();
            return View(chamCongs);
        }

        // Create chấm công
        public IActionResult CreateChamCong()
        {
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
            return View();
        }

        // POST: ChamCong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChamCong([Bind("MaChamCong,MaNhanVien,NgayChamCong,CheckIn,CheckOut,TrangThai")] Chamcong chamCong)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage); // Ghi ra lỗi
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tắt validation cho MaNhanVienNavigation
                    ModelState.Remove("MaNhanVienNavigation");

                    // Kiểm tra xem mã nhân viên có tồn tại không
                    var nhanVien = await _context.Nhanviens.FindAsync(chamCong.MaNhanVien);
                    if (nhanVien == null)
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên không tồn tại.");
                        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", chamCong.MaNhanVien);
                        return View(chamCong);
                    }

                    // Kiểm tra và chuyển đổi ngày tháng hợp lệ
                    if (chamCong.NgayChamCong < new DateTime(1753, 1, 1) || chamCong.NgayChamCong > DateTime.MaxValue)
                    {
                        ModelState.AddModelError("NgayChamCong", "Ngày chấm công không hợp lệ. Phải lớn hơn hoặc bằng 1/1/1753.");
                    }

                    if (chamCong.CheckIn.HasValue && (chamCong.CheckIn.Value < new DateTime(1753, 1, 1) || chamCong.CheckIn.Value > DateTime.MaxValue))
                    {
                        ModelState.AddModelError("CheckIn", "Giờ vào không hợp lệ. Phải lớn hơn hoặc bằng 1/1/1753.");
                    }

                    if (chamCong.CheckOut.HasValue && (chamCong.CheckOut.Value < new DateTime(1753, 1, 1) || chamCong.CheckOut.Value > DateTime.MaxValue))
                    {
                        ModelState.AddModelError("CheckOut", "Giờ ra không hợp lệ. Phải lớn hơn hoặc bằng 1/1/1753.");
                    }

                    if (ModelState.IsValid)
                    {
                        // Gán MaNhanVienNavigation cho chấm công
                        chamCong.MaNhanVienNavigation = nhanVien; // Gán đối tượng Nhanvien vào thuộc tính MaNhanVienNavigation

                        // Khởi tạo giá trị cho MaChamCong nếu cần thiết
                        if (string.IsNullOrEmpty(chamCong.MaChamCong))
                        {
                            chamCong.MaChamCong = Guid.NewGuid().ToString();
                        }

                        // Thêm chấm công vào cơ sở dữ liệu
                        _context.Add(chamCong);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(ListChamCong)); // Điều hướng đến danh sách chấm công
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
            }

            // Nếu có lỗi, truyền lại danh sách nhân viên cho View
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", chamCong.MaNhanVien);
            return View(chamCong);
        }

        // Sửa Chấm Công
        // Hiển thị form sửa chấm công
        [HttpGet]
        public async Task<IActionResult> EditChamCong(string maChamCong)
        {
            Console.WriteLine("Starting EditChamCong method...");

            if (string.IsNullOrEmpty(maChamCong))
            {
                Console.WriteLine("ID is null or empty");
                return NotFound();
            }

            var chamCong = await _context.Chamcongs.FirstOrDefaultAsync(m => m.MaChamCong == maChamCong);

            if (chamCong == null)
            {
                Console.WriteLine("ChamCong not found");
                return NotFound();
            }

            // Truyền danh sách nhân viên vào ViewBag
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", chamCong.MaNhanVien);

            return View(chamCong); // Trả về view để người dùng có thể sửa chấm công
        }

        // Xử lý form sửa chấm công
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChamCong(string maChamCong, [Bind("MaChamCong,MaNhanVien,NgayChamCong,CheckIn,CheckOut,TrangThai")] Chamcong chamCong)
        {
            Console.WriteLine("Starting EditChamCong method...");

            if (maChamCong != chamCong.MaChamCong)
            {
                Console.WriteLine($"ID mismatch: {maChamCong} != {chamCong.MaChamCong}");
                return NotFound();
            }

            try
            {
                // Tắt validation cho các navigation properties nếu có
                ModelState.Remove("MaNhanVienNavigation");

                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid.");

                    // Kiểm tra xem chấm công đã tồn tại chưa
                    var existingChamCong = await _context.Chamcongs.FindAsync(chamCong.MaChamCong);
                    if (existingChamCong == null)
                    {
                        Console.WriteLine("ChamCong không tồn tại.");
                        return NotFound();
                    }

                    // Cập nhật các trường của existingChamCong từ chamCong
                    existingChamCong.MaNhanVien = chamCong.MaNhanVien;
                    existingChamCong.NgayChamCong = chamCong.NgayChamCong;
                    existingChamCong.CheckIn = chamCong.CheckIn;
                    existingChamCong.CheckOut = chamCong.CheckOut;
                    existingChamCong.TrangThai = chamCong.TrangThai;

                    // Đánh dấu entity là đã được thay đổi
                    _context.Update(existingChamCong);

                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("ChamCong đã được cập nhật thành công.");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!ChamCongExists(chamCong.MaChamCong))
                        {
                            Console.WriteLine("ChamCong không tồn tại khi lưu.");
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

                    return RedirectToAction(nameof(ListChamCong)); // Điều hướng đến danh sách chấm công
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

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", chamCong.MaNhanVien);
            return View(chamCong);
        }

        // Kiểm tra xem chấm công có tồn tại trong cơ sở dữ liệu hay không
        private bool ChamCongExists(string maChamCong)
        {
            return _context.Chamcongs.Any(e => e.MaChamCong == maChamCong);
        }

        // Delete chấm công
        public async Task<IActionResult> DeleteChamCong(string maChamCong)
        {
            if (maChamCong == null)
            {
                return NotFound();
            }

            var chamCong = await _context.Chamcongs
                .FirstOrDefaultAsync(m => m.MaChamCong == maChamCong);
            if (chamCong == null)
            {
                return NotFound();
            }

            return View(chamCong);
        }


        [HttpPost, ActionName("DeleteChamCong")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChamCongConfirmed(string maChamCong)
        {
            var chamCong = await _context.Chamcongs.FindAsync(maChamCong);
            if (chamCong == null)
            {
                return NotFound();
            }

            _context.Chamcongs.Remove(chamCong);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListChamCong));
        }
    }
}
