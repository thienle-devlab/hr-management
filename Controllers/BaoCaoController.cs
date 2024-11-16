using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly HrmContext _context;

        public BaoCaoController(HrmContext context)
        {
            _context = context;
        }

        // GET: BaoCao
        public IActionResult ListBC()
        {
            var baocaos = _context.Baocaos.ToList();
            return View(baocaos);
        }

        // Controller Tạo Báo Cáo

        // Hiển thị form tạo báo cáo
        [HttpGet]
        public IActionResult CreateBaoCao()
        {
            // Truyền danh sách nhân viên vào ViewBag
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");

            // Trả về form tạo báo cáo
            return View();
        }

        // Xử lý form tạo báo cáo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBaoCao([Bind("MaBaoCao,MaNhanVien,TenBaoCao,DuLieuBaoCao,NgayTaoBaoCao,LoaiBaoCao,TrangThaiBaoCao")] Baocao baocao)
        {
            // Kiểm tra model state hợp lệ
            if (ModelState.IsValid)
            {
                try
                {
                    // Tắt validation cho MaNhanVienNavigation
                    ModelState.Remove("MaNhanVienNavigation");

                    // Kiểm tra xem mã nhân viên có tồn tại không
                    var nhanVien = await _context.Nhanviens.FindAsync(baocao.MaNhanVien);
                    if (nhanVien == null)
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên không tồn tại.");
                        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", baocao.MaNhanVien);
                        return View(baocao);
                    }

                    // Gán MaNhanVienNavigation cho báo cáo
                    baocao.MaNhanVienNavigation = nhanVien;

                    // Tạo mã báo cáo nếu không có
                    if (string.IsNullOrEmpty(baocao.MaBaoCao))
                    {
                        baocao.MaBaoCao = Guid.NewGuid().ToString();
                    }

                    // Cập nhật ngày tạo báo cáo
                    baocao.NgayTaoBaoCao = DateTime.Now;

                    // Thêm báo cáo vào cơ sở dữ liệu
                    _context.Add(baocao);
                    await _context.SaveChangesAsync();

                    // Điều hướng đến danh sách báo cáo
                    return RedirectToAction(nameof(ListBC));
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu có
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo báo cáo.");
                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
            }

            // Nếu model không hợp lệ, trả lại form tạo báo cáo
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", baocao.MaNhanVien);
            return View(baocao);
        }

        // Controller Sửa Báo Cáo

        // Hiển thị form sửa báo cáo
        [HttpGet]
        public async Task<IActionResult> EditBaoCao(string maBaoCao)
        {
            Console.WriteLine("Starting EditBaoCao method...");

            if (string.IsNullOrEmpty(maBaoCao))
            {
                Console.WriteLine("ID is null or empty");
                return NotFound();
            }

            var baoCao = await _context.Baocaos.FirstOrDefaultAsync(m => m.MaBaoCao == maBaoCao);

            if (baoCao == null)
            {
                Console.WriteLine("BaoCao not found");
                return NotFound();
            }

            // Truyền danh sách nhân viên vào ViewBag
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", baoCao.MaNhanVien);

            return View(baoCao); // Trả về view để người dùng có thể sửa báo cáo
        }

        // Xử lý form sửa báo cáo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBaoCao(string maBaoCao, [Bind("MaBaoCao,MaNhanVien,TenBaoCao,DuLieuBaoCao,NgayTaoBaoCao,LoaiBaoCao,TrangThaiBaoCao")] Baocao baoCao)
        {
            Console.WriteLine("Starting EditBaoCao method...");

            if (maBaoCao != baoCao.MaBaoCao)
            {
                Console.WriteLine($"ID mismatch: {maBaoCao} != {baoCao.MaBaoCao}");
                return NotFound();
            }

            try
            {
                // Tắt validation cho các navigation properties nếu có
                ModelState.Remove("MaNhanVienNavigation");

                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid.");

                    // Kiểm tra xem báo cáo đã tồn tại chưa
                    var existingBaoCao = await _context.Baocaos.FindAsync(baoCao.MaBaoCao);
                    if (existingBaoCao == null)
                    {
                        Console.WriteLine("BaoCao không tồn tại.");
                        return NotFound();
                    }

                    // Cập nhật các trường của existingBaoCao từ baoCao
                    existingBaoCao.MaNhanVien = baoCao.MaNhanVien;
                    existingBaoCao.TenBaoCao = baoCao.TenBaoCao;
                    existingBaoCao.DuLieuBaoCao = baoCao.DuLieuBaoCao;
                    existingBaoCao.LoaiBaoCao = baoCao.LoaiBaoCao;
                    existingBaoCao.TrangThaiBaoCao = baoCao.TrangThaiBaoCao;

                    // Đánh dấu entity là đã được thay đổi
                    _context.Update(existingBaoCao);

                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("BaoCao đã được cập nhật thành công.");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!BaoCaoExists(baoCao.MaBaoCao))
                        {
                            Console.WriteLine("BaoCao không tồn tại khi lưu.");
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

                    return RedirectToAction(nameof(ListBC)); // Điều hướng đến danh sách báo cáo
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

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", baoCao.MaNhanVien);
            return View(baoCao);
        }

        // Kiểm tra xem báo cáo có tồn tại trong cơ sở dữ liệu hay không
        private bool BaoCaoExists(string maBaoCao)
        {
            return _context.Baocaos.Any(e => e.MaBaoCao == maBaoCao);
        }

        // Delete BC
        public async Task<IActionResult> DeleteBaoCao(string maBaoCao)
        {
            if (maBaoCao == null)
            {
                return NotFound();
            }

            var baoCao = await _context.Baocaos
                .FirstOrDefaultAsync(m => m.MaBaoCao == maBaoCao);
            if (baoCao == null)
            {
                return NotFound();
            }

            return View(baoCao);
        }


        [HttpPost, ActionName("DeleteBaoCao")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBaoCaoConfirmed(string maBaoCao)
        {
            var baoCao = await _context.Baocaos.FindAsync(maBaoCao);
            if (baoCao == null)
            {
                return NotFound();
            }

            _context.Baocaos.Remove(baoCao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListBC));
        }

    }
}
