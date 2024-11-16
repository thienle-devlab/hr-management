using HR_Management.Class;
using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly HrmContext _context;

        // Constructor
        public NhanVienController(HrmContext context)
        {
            _context = context;
        }

        // Lấy danh sách Nhân viên
        [CustomAuth(maChucVu: new string[] { "CV002" }, maPhongBan: new string[] { "PB002" })] // Phân quyền
        public async Task<IActionResult> ListNV()
        {
            var nv = await _context.Nhanviens.ToListAsync();
            return View(nv);
        }

        // Thêm nhân viên
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public async Task <IActionResult> CreateNV()
        {
            // Tạo danh sách chức vụ để hiển thị trong view
            ViewBag.ChucVuList = new SelectList(await _context.Chucvus.ToListAsync(), "MaChucVu", "TenChucVu");
            return View(new Nhanvien());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNV(Nhanvien nhanVien)
        {
            try
            {
                // Log dữ liệu trước khi insert
                Console.WriteLine($"MaNhanVien: {nhanVien.MaNhanVien}");
                Console.WriteLine($"TenNhanVien: {nhanVien.TenNhanVien}");
                Console.WriteLine($"NgaySinh: {nhanVien.NgaySinh}");
                Console.WriteLine($"CCCD: {nhanVien.Cccd}");
                Console.WriteLine($"MaChucVu: {nhanVien.MaChucVu}");
                Console.WriteLine($"AnhDaiDien: {nhanVien.AnhDaiDien}");

                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                    // Ghi lại danh sách chức vụ
                    ViewBag.ChucVuList = new SelectList(await _context.Chucvus.ToListAsync(), "MaChucVu", "TenChucVu");
                    return View(nhanVien);
                }

                // Xử lý file ảnh
                if (nhanVien.AnhDaiDienFile != null && nhanVien.AnhDaiDienFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(nhanVien.AnhDaiDienFile.FileName)}";
                    var filePath = Path.Combine("Assets", "Images", fileName);
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);

                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await nhanVien.AnhDaiDienFile.CopyToAsync(stream);
                        }
                        nhanVien.AnhDaiDien = $"/{filePath.Replace("\\", "/")}";
                        Console.WriteLine($"Ảnh đã được lưu tại: {fullPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving file: {ex.Message}");
                        ModelState.AddModelError("", "Có lỗi khi lưu ảnh đại diện: " + ex.Message);
                        // Ghi lại danh sách chức vụ
                        ViewBag.ChucVuList = new SelectList(await _context.Chucvus.ToListAsync(), "MaChucVu", "TenChucVu");
                        return View(nhanVien);
                    }
                }

                // Kiểm tra tồn tại của MaChucVu
                var chucVu = await _context.Chucvus.FindAsync(nhanVien.MaChucVu);
                if (chucVu == null)
                {
                    ModelState.AddModelError("MaChucVu", "Chức vụ không tồn tại");
                    ViewBag.ChucVuList = new SelectList(await _context.Chucvus.ToListAsync(), "MaChucVu", "TenChucVu");
                    return View(nhanVien);
                }

                // Thêm nhân viên vào cơ sở dữ liệu
                _context.Nhanviens.Add(nhanVien);
                await _context.SaveChangesAsync();
                Console.WriteLine("Nhân viên đã được lưu vào cơ sở dữ liệu");

                return RedirectToAction(nameof(ListNV)); // Chuyển hướng về danh sách nhân viên
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

            // Nếu có lỗi, ghi lại danh sách chức vụ và trả về view
            ViewBag.ChucVuList = new SelectList(await _context.Chucvus.ToListAsync(), "MaChucVu", "TenChucVu");
            return View(nhanVien);
        }

        // Cập nhật nhân viên
        public async Task<IActionResult> EditNV(string maNhanVien)
        {
            var nhanVien = await _context.Nhanviens.FindAsync(maNhanVien);
            if (nhanVien == null)
            {
                return NotFound();
            }
            ViewBag.ChucVuList = new SelectList(await _context.Chucvus.ToListAsync(), "MaChucVu", "TenChucVu", nhanVien.MaChucVu);
            return View(nhanVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNV(Nhanvien nhanVien)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ChucVuList = new SelectList(await _context.Chucvus.ToListAsync(), "MaChucVu", "TenChucVu", nhanVien.MaChucVu);
                    return View(nhanVien);
                }

                // Lấy nhân viên hiện tại từ cơ sở dữ liệu
                var existingNhanVien = await _context.Nhanviens.AsNoTracking().FirstOrDefaultAsync(nv => nv.MaNhanVien == nhanVien.MaNhanVien);
                if (existingNhanVien == null)
                {
                    return NotFound();
                }

                // Xử lý file ảnh nếu có
                if (nhanVien.AnhDaiDienFile != null && nhanVien.AnhDaiDienFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(nhanVien.AnhDaiDienFile.FileName)}";
                    var filePath = Path.Combine("Assets", "Images", fileName);
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);

                    // Xóa ảnh cũ (nếu cần)
                    if (!string.IsNullOrEmpty(nhanVien.AnhDaiDien))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", nhanVien.AnhDaiDien.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    // Lưu ảnh mới
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await nhanVien.AnhDaiDienFile.CopyToAsync(stream);
                    }
                    nhanVien.AnhDaiDien = $"/{filePath.Replace("\\", "/")}";
                }
                else
                {
                    // Gán lại ảnh cũ nếu không có ảnh mới
                    nhanVien.AnhDaiDien = existingNhanVien.AnhDaiDien;
                }
                // Cập nhật thông tin nhân viên
                _context.Nhanviens.Update(nhanVien);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListNV));
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

            // Nếu có lỗi, ghi lại danh sách chức vụ và trả về view
            ViewBag.ChucVuList = new SelectList(await _context.Chucvus.ToListAsync(), "MaChucVu", "TenChucVu", nhanVien.MaChucVu);
            return View(nhanVien);
        }

        // Xóa nhân viên
        public async Task<IActionResult> DeleteNV(string maNhanVien)
        {
            var nhanVien = await _context.Nhanviens.FindAsync(maNhanVien);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View("DeleteConfirm", nhanVien); // Trả về view xác nhận xóa
        }

        [HttpPost, ActionName("DeleteNV")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNVConfirmed(string maNhanVien)
        {
            var nhanVien = await _context.Nhanviens.FindAsync(maNhanVien);
            if (nhanVien == null)
            {
                return NotFound();
            }

            try
            {
                // Xóa file ảnh đại diện nếu có
                if (!string.IsNullOrEmpty(nhanVien.AnhDaiDien))
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", nhanVien.AnhDaiDien.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                // Xóa nhân viên khỏi cơ sở dữ liệu
                _context.Nhanviens.Remove(nhanVien);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListNV));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            return View(nhanVien);
        }
    }
}
