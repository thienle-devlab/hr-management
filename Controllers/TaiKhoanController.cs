using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HR_Management.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly HrmContext _context;

        // Constructor
        public TaiKhoanController(HrmContext context)
        {
            _context = context;
        }

        // Lấy danh sách Tài khoản
        public async Task<IActionResult> ListTaiKhoan()
        {
            var tk = await _context.Taikhoans.ToListAsync();
            return View(tk);
        }

        // Tạo tài khoản
        public IActionResult Register()
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
        public async Task<IActionResult> Register(Taikhoan taiKhoan)
        {
            try
            {
                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    // Ghi lại lỗi
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Lỗi ModelState: {error.ErrorMessage}");
                    }

                    // Ghi lại danh sách nhân viên
                    ViewBag.NhanVienList = new SelectList(await _context.Nhanviens.ToListAsync(), "MaNhanVien", "TenNhanVien");
                    return View(taiKhoan);
                }

                // Kiểm tra tồn tại của Nhân viên
                Console.WriteLine($"Mã Nhân Viên: {taiKhoan.MaNhanVien}");
                var nhanVien = await _context.Nhanviens.FindAsync(taiKhoan.MaNhanVien);
                if (nhanVien == null)
                {
                    ModelState.AddModelError("MaNhanVien", "Nhân viên không tồn tại");
                    ViewBag.NhanVienList = new SelectList(await _context.Nhanviens.ToListAsync(), "MaNhanVien", "TenNhanVien");
                    return View(taiKhoan);
                }

                // Băm mật khẩu
                taiKhoan.Password = HashPassword(taiKhoan.Password);
                Console.WriteLine($"Băm mật khẩu: {taiKhoan.Password}");

                // Thêm tài khoản vào cơ sở dữ liệu
                _context.Taikhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();

                Console.WriteLine("Tài khoản đã được lưu vào cơ sở dữ liệu.");
                return RedirectToAction(nameof(ListTaiKhoan));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            // Nếu có lỗi, ghi lại danh sách nhân viên và trả về view
            ViewBag.NhanVienList = new SelectList(await _context.Nhanviens.ToListAsync(), "MaNhanVien", "TenNhanVien");
            return View(taiKhoan);
        }

        // Hàm băm mật khẩu
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // Chỉnh sửa tài khoản
        public async Task<IActionResult> EditTaiKhoan(string userName)
        {
            if (userName == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.Taikhoans.FindAsync(userName);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            ViewBag.NhanVienList = new SelectList(await _context.Nhanviens.ToListAsync(), "MaNhanVien", "TenNhanVien", taiKhoan.MaNhanVien);
            return View(taiKhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTaiKhoan(string userName, Taikhoan taiKhoan, string currentPasswordHash)
        {
            if(userName != taiKhoan.Username)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAccount = await _context.Taikhoans.AsNoTracking().FirstOrDefaultAsync(t => t.Username == userName);

                    if (existingAccount == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra xem mật khẩu có thay đổi không
                    if (!string.IsNullOrEmpty(taiKhoan.Password) && taiKhoan.Password != currentPasswordHash)
                    {
                        taiKhoan.Password = HashPassword(taiKhoan.Password);
                    }
                    else
                    {
                        // Giữ nguyên mật khẩu cũ nếu không thay đổi
                        taiKhoan.Password = currentPasswordHash;
                    }

                    _context.Update(taiKhoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaikhoanExists(taiKhoan.Username))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListTaiKhoan));
            }
            ViewBag.NhanVienList = new SelectList(await _context.Nhanviens.ToListAsync(), "MaNhanVien", "TenNhanVien", taiKhoan.MaNhanVien);
            return View(taiKhoan);
        }

        // Kiểm tra tài khoản
        private bool TaikhoanExists(string id)
        {
            return _context.Taikhoans.Any(e => e.Username == id);
        }

        // Xóa tài khoản
        public async Task<IActionResult> DeleteTaiKhoan(string userName)
        {
            if (userName == null)
            {
                return NotFound();
            }
            
            var taiKhoan = await _context.Taikhoans.FirstOrDefaultAsync(m => m.Username == userName);
            if(taiKhoan == null)
            {
                return NotFound();
            }
            return View(taiKhoan);
        }

        [HttpPost, ActionName("DeleteTaiKhoan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string userName)
        {
            var taiKhoan = await _context.Taikhoans.FindAsync(userName);
            if (taiKhoan != null)
            {
                _context.Taikhoans.Remove(taiKhoan);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListTaiKhoan));
        }
    }
}
