using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HR_Management.Controllers
{
    public class AuthController : Controller
    {
        private readonly HrmContext _context;

        public AuthController(HrmContext context)
        {
            _context = context;
        }

        // Đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var hashedPassword = HashPassword(password);
            var taiKhoan = await _context.Taikhoans.FirstOrDefaultAsync(tk => tk.Username == username && tk.Password == hashedPassword);

            if (taiKhoan != null)
            {
                // Lưu MaNhanVien vào Session
                HttpContext.Session.SetString("MaNhanVien", taiKhoan.MaNhanVien);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng");
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
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
    }
}
