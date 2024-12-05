using HR_Management.Class;
using HR_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management.Controllers
{
    public class HopDongController : Controller
    {
        private readonly HrmContext _context;

        public HopDongController(HrmContext context)
        {
            _context = context;
        }

        // List Hợp Đồng
        public async Task<IActionResult> ListHD()
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
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
                return RedirectToAction("Auth", "Login");
            }

            bool isAdmin = nhanVien.MaChucVu == "CV001" && nhanVien.MaPhongBan == "PB001";
            string userPhongBan = nhanVien.MaPhongBan;

            // Kiểm tra quyền admin: Nếu là admin (CV001, PB001) thì cho phép xem tất cả tài khoản
            if (isAdmin)
            {
                var allHD = await _context.HopDongs.ToListAsync();
                ViewBag.IsAdmin = true;
                return View(allHD);
            }

            // Nếu không phải admin, chỉ cho phép xem tài khoản thuộc cùng phòng ban
            var hdInSamePhongBan = await _context.HopDongs
                .Include(hd => hd.MaNhanVienNavigation)
                .Where(hd => hd.MaNhanVienNavigation.MaChucVuNavigation.MaPhongBan == userPhongBan)
                .ToListAsync();

            ViewBag.IsAdmin = false;
            return View(hdInSamePhongBan);
            //var hopdongs = _context.HopDongs.ToList();
            //return View(hopdongs);
        }

        // Hiển thị form tạo hợp đồng
    [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
    public IActionResult CreateHD()
    {
        var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
        if (string.IsNullOrEmpty(maNhanVien))
        {
            return RedirectToAction("Login", "Auth");
        }
        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien");
        return View();
    }

    // POST: HopDong/CreateHD
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateHD([Bind("MaHopDong,MaNhanVien,LoaiHopDong,NgayKy,NgayBatDau,NgayKetThuc,TrangThai,MucLuong")] Hopdong hopDong)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Tắt validation cho MaNhanVienNavigation
                ModelState.Remove("MaNhanVienNavigation");

                // Kiểm tra xem mã nhân viên có tồn tại không
                var nhanVien = await _context.Nhanviens.FindAsync(hopDong.MaNhanVien);
                if (nhanVien == null)
                {
                    ModelState.AddModelError("MaNhanVien", "Mã nhân viên không tồn tại.");
                }

                // Kiểm tra và chuyển đổi ngày tháng hợp lệ
                if (hopDong.NgayKy < new DateTime(1753, 1, 1) || hopDong.NgayKy > DateTime.MaxValue)
                {
                    ModelState.AddModelError("NgayKy", "Ngày ký không hợp lệ. Phải lớn hơn hoặc bằng 1/1/1753.");
                }

                if (hopDong.NgayBatDau < new DateTime(1753, 1, 1) || hopDong.NgayBatDau > DateTime.MaxValue)
                {
                    ModelState.AddModelError("NgayBatDau", "Ngày bắt đầu không hợp lệ. Phải lớn hơn hoặc bằng 1/1/1753.");
                }

                if (hopDong.NgayKetThuc.HasValue && (hopDong.NgayKetThuc.Value < new DateTime(1753, 1, 1) || hopDong.NgayKetThuc.Value > DateTime.MaxValue))
                {
                    ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc không hợp lệ. Phải lớn hơn hoặc bằng 1/1/1753.");
                }

                if (ModelState.IsValid)
                {
                    // Gán MaNhanVienNavigation cho hợp đồng
                    hopDong.MaNhanVienNavigation = nhanVien;

                    // Thêm hợp đồng vào cơ sở dữ liệu
                    _context.Add(hopDong);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(ListHD)); // Điều hướng đến danh sách hợp đồng
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
        ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", hopDong.MaNhanVien);
        return View(hopDong);
        }

        //Sửa Hợp đồng
        // Hiển thị form sửa hợp đồng
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        [HttpGet]
        public async Task<IActionResult> EditHD(string maHopDong)
        {
            Console.WriteLine("Starting EditHD method...");

            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(maHopDong))
            {
                Console.WriteLine("ID is null or empty");
                return NotFound();
            }

            var hopDong = await _context.HopDongs.FirstOrDefaultAsync(m => m.MaHopDong == maHopDong);

            if (hopDong == null)
            {
                Console.WriteLine("HopDong not found");
                return NotFound();
            }

            // Truyền danh sách nhân viên vào ViewBag
            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", hopDong.MaNhanVien);

            return View(hopDong); // Trả về view để người dùng có thể sửa hợp đồng
        }

        // Xử lý form sửa hợp đồng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHD(string maHopDong, [Bind("MaHopDong,MaNhanVien,LoaiHopDong,NgayKy,NgayBatDau,NgayKetThuc,TrangThai,MucLuong")] Hopdong hopDong)
        {
            Console.WriteLine("Starting EditHD method...");

            if (maHopDong != hopDong.MaHopDong)
            {
                Console.WriteLine($"ID mismatch: {maHopDong} != {hopDong.MaHopDong}");
                return NotFound();
            }

            try
            {
                // Tắt validation cho các navigation properties nếu có
                ModelState.Remove("MaNhanVienNavigation");

                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid.");

                    // Kiểm tra xem hợp đồng đã tồn tại chưa
                    var existingHopDong = await _context.HopDongs.FindAsync(hopDong.MaHopDong);
                    if (existingHopDong == null)
                    {
                        Console.WriteLine("HopDong không tồn tại.");
                        return NotFound();
                    }

                    // Cập nhật các trường của existingHopDong từ hopDong
                    existingHopDong.MaNhanVien = hopDong.MaNhanVien;
                    existingHopDong.LoaiHopDong = hopDong.LoaiHopDong;
                    existingHopDong.NgayKy = hopDong.NgayKy;
                    existingHopDong.NgayBatDau = hopDong.NgayBatDau;
                    existingHopDong.NgayKetThuc = hopDong.NgayKetThuc;
                    existingHopDong.TrangThai = hopDong.TrangThai;
                    existingHopDong.MucLuong = hopDong.MucLuong;

                    // Đánh dấu entity là đã được thay đổi
                    _context.Update(existingHopDong);

                    try
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("HopDong đã được cập nhật thành công.");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!HopDongExists(hopDong.MaHopDong))
                        {
                            Console.WriteLine("HopDong không tồn tại khi lưu.");
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

                    return RedirectToAction(nameof(ListHD)); // Điều hướng đến danh sách hợp đồng
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

            ViewBag.NhanVienList = new SelectList(_context.Nhanviens, "MaNhanVien", "TenNhanVien", hopDong.MaNhanVien);
            return View(hopDong);
        }


        // Kiểm tra xem hợp đồng có tồn tại trong cơ sở dữ liệu hay không
        private bool HopDongExists(string maHopDong)
        {
            return _context.HopDongs.Any(e => e.MaHopDong == maHopDong);
        }

        // Delete hợp đồng
        // GET: HopDong/DeleteHD/5
        [CustomAuth(maChucVu: new string[] { "CV001" }, maPhongBan: new string[] { "PB001" })]
        public async Task<IActionResult> DeleteHD(string maHopDong)
        {
            var maNhanVien = HttpContext.Session.GetString("MaNhanVien");
            if (string.IsNullOrEmpty(maNhanVien))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (maHopDong == null)
            {
                return NotFound();
            }

            var hopDong = await _context.HopDongs
                .FirstOrDefaultAsync(m => m.MaHopDong == maHopDong);
            if (hopDong == null)
            {
                return NotFound();
            }

            return View(hopDong);
        }


        // POST: HopDong/DeleteHD/5
        [HttpPost, ActionName("DeleteHD")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHDConfirmed(string maHopDong)
        {
            var hopDong = await _context.HopDongs.FindAsync(maHopDong);
            if (hopDong == null)
            {
                return NotFound();
            }

            _context.HopDongs.Remove(hopDong);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListHD));
        }


    }
}
