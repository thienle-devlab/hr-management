using HR_Management.Class;
using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

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
        [HttpGet]
        public async Task<IActionResult> ListNV()
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin nhân viên từ DB để kiểm tra quyền
            var nhanVien = await _context.Nhanviens
                .Include(nv => nv.MaChucVuNavigation)
                .ThenInclude(cv => cv.MaPhongBanNavigation)
                .Where(nv => nv.MaNhanVien == maNhanVien)
                .Select(nv => new
                {
                    nv.MaChucVu,
                    nv.MaChucVuNavigation.MaPhongBan
                })
                .FirstOrDefaultAsync();

            if (nhanVien == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            bool isAdmin = nhanVien.MaChucVu == "CV001" && nhanVien.MaPhongBan == "PB001";
            string userPhongBan = nhanVien.MaPhongBan;

            // Kiểm tra quyền admin: Nếu là admin (CV001, PB001) thì cho phép xem tất cả nhân viên
            if (isAdmin)
            {
                var allNV = await _context.Nhanviens.ToListAsync();
                ViewBag.IsAdmin = true;
                return View(allNV);
            }

            // Nếu không phải admin, chỉ cho phép xem nhân viên thuộc cùng phòng ban
            var nvInSamePhongBan = await _context.Nhanviens
                .Where(nv => nv.MaChucVuNavigation != null && nv.MaChucVuNavigation.MaPhongBan == userPhongBan)
                .ToListAsync();

            ViewBag.IsAdmin = false;
            return View(nvInSamePhongBan);
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
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
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
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
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

        // Hành động xuất file Excel
        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin nhân viên từ DB để kiểm tra quyền
            var nhanVien = await _context.Nhanviens
                .Include(nv => nv.MaChucVuNavigation)
                .ThenInclude(cv => cv.MaPhongBanNavigation)
                .Where(nv => nv.MaNhanVien == maNhanVien)
                .Select(nv => new
                {
                    nv.MaChucVu,
                    nv.MaChucVuNavigation.MaPhongBan
                })
                .FirstOrDefaultAsync();

            if (nhanVien == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            bool isAdmin = nhanVien.MaChucVu == "CV001" && nhanVien.MaPhongBan == "PB001";
            string userPhongBan = nhanVien.MaPhongBan;

            List<Nhanvien> employeesToExport;

            if (isAdmin)
            {
                employeesToExport = await _context.Nhanviens.ToListAsync();
            }
            else
            {
                employeesToExport = await _context.Nhanviens
                    .Where(nv => nv.MaChucVuNavigation != null && nv.MaChucVuNavigation.MaPhongBan == userPhongBan)
                    .ToListAsync();
            }

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh sách nhân viên");

                // Đặt tiêu đề cột
                worksheet.Cells[1, 1].Value = "Mã Nhân Viên";
                worksheet.Cells[1, 2].Value = "Tên Nhân Viên";
                worksheet.Cells[1, 3].Value = "Ngày Sinh";
                worksheet.Cells[1, 4].Value = "Giới Tính";
                worksheet.Cells[1, 5].Value = "Ảnh Đại Diện";
                worksheet.Cells[1, 6].Value = "Địa Chỉ";
                worksheet.Cells[1, 7].Value = "Số Điện Thoại";
                worksheet.Cells[1, 8].Value = "Email";
                worksheet.Cells[1, 9].Value = "CCCD";
                worksheet.Cells[1, 10].Value = "Mã Chức Vụ";

                // Điền dữ liệu
                int row = 2;
                foreach (var nv in employeesToExport)
                {
                    worksheet.Cells[row, 1].Value = nv.MaNhanVien;
                    worksheet.Cells[row, 2].Value = nv.TenNhanVien;
                    worksheet.Cells[row, 3].Value = nv.NgaySinh.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = nv.GioiTinh;
                    worksheet.Cells[row, 5].Value = nv.AnhDaiDien;
                    worksheet.Cells[row, 6].Value = nv.DiaChi;
                    worksheet.Cells[row, 7].Value = nv.SoDienThoai;
                    worksheet.Cells[row, 8].Value = nv.Email;
                    worksheet.Cells[row, 9].Value = nv.Cccd;
                    worksheet.Cells[row, 10].Value = nv.MaChucVu;

                    row++;
                }

                // Trả về file Excel
                var fileContent = package.GetAsByteArray();
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachNhanVien.xlsx");
            }
        }
    }
}
