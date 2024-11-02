using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddHopDong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CONGVIEC",
                columns: table => new
                {
                    MaCongViec = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    TenCongViec = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTaCongViec = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CONGVIEC__41B7DD18EF670E57", x => x.MaCongViec);
                });

            migrationBuilder.CreateTable(
                name: "KHAUTRU",
                columns: table => new
                {
                    MaKhauTru = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    TenKhauTru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KHAUTRU__B19C10BB97EAFE8A", x => x.MaKhauTru);
                });

            migrationBuilder.CreateTable(
                name: "PHONGBAN",
                columns: table => new
                {
                    MaPhongBan = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    TenPhongBan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PHONGBAN__D0910CC8D68F8DD0", x => x.MaPhongBan);
                });

            migrationBuilder.CreateTable(
                name: "PHUCAP",
                columns: table => new
                {
                    MaPhuCap = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    TenPhuCap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PHUCAP__F97D973833EAD57F", x => x.MaPhuCap);
                });

            migrationBuilder.CreateTable(
                name: "CHUCVU",
                columns: table => new
                {
                    MaChucVu = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    TenChucVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaPhongBan = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CHUCVU__D4639533A2099E28", x => x.MaChucVu);
                    table.ForeignKey(
                        name: "FK_CV_PB",
                        column: x => x.MaPhongBan,
                        principalTable: "PHONGBAN",
                        principalColumn: "MaPhongBan");
                });

            migrationBuilder.CreateTable(
                name: "NHANVIEN",
                columns: table => new
                {
                    MaNhanVien = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    TenNhanVien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: false),
                    GioiTinh = table.Column<bool>(type: "bit", nullable: false),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoDienThoai = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    CCCD = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: false),
                    MaChucVu = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NHANVIEN__77B2CA4719FD023B", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_NV_CV",
                        column: x => x.MaChucVu,
                        principalTable: "CHUCVU",
                        principalColumn: "MaChucVu");
                });

            migrationBuilder.CreateTable(
                name: "BAOCAO",
                columns: table => new
                {
                    MaBaoCao = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    TenBaoCao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DuLieuBaoCao = table.Column<string>(type: "text", nullable: false),
                    NgayTaoBaoCao = table.Column<DateTime>(type: "datetime", nullable: false),
                    LoaiBaoCao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThaiBaoCao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BAOCAO__25A9188C5B7DE56C", x => x.MaBaoCao);
                    table.ForeignKey(
                        name: "FK_BC_NV",
                        column: x => x.MaNhanVien,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "CHAMCONG",
                columns: table => new
                {
                    MaChamCong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    NgayChamCong = table.Column<DateTime>(type: "datetime", nullable: false),
                    CheckIn = table.Column<DateTime>(type: "datetime", nullable: true),
                    CheckOut = table.Column<DateTime>(type: "datetime", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CHAMCONG__307331A18DDF1E50", x => x.MaChamCong);
                    table.ForeignKey(
                        name: "FK_CC_NV",
                        column: x => x.MaNhanVien,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "HOPDONG",
                columns: table => new
                {
                    MaHopDong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    NgayKy = table.Column<DateTime>(type: "datetime", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime", nullable: true),
                    LoaiHopDong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MucLuong = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HOPDONG", x => x.MaHopDong);
                    table.ForeignKey(
                        name: "FK_HD_NV",
                        column: x => x.MaNhanVien,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "LUONG",
                columns: table => new
                {
                    MaLuong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    LuongCoBan = table.Column<int>(type: "int", nullable: false),
                    NgayNhanLuong = table.Column<DateTime>(type: "datetime", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LUONG__6609A48DD7BCC558", x => x.MaLuong);
                    table.ForeignKey(
                        name: "FK_L_NV",
                        column: x => x.MaNhanVien,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "NGHIPHEP",
                columns: table => new
                {
                    MaNghiPhep = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime", nullable: false),
                    LyDo = table.Column<string>(type: "text", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NGHIPHEP__1ED7CDB4BD94D157", x => x.MaNghiPhep);
                    table.ForeignKey(
                        name: "FK_NP_NV",
                        column: x => x.MaNhanVien,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "PHANCONG",
                columns: table => new
                {
                    MaPhanCong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaCongViec = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PHANCONG__C279D91611211C01", x => x.MaPhanCong);
                    table.ForeignKey(
                        name: "FK_PC_CV",
                        column: x => x.MaCongViec,
                        principalTable: "CONGVIEC",
                        principalColumn: "MaCongViec");
                    table.ForeignKey(
                        name: "FK_PC_NV",
                        column: x => x.MaNhanVien,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "TAIKHOAN",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNhanVien = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TAIKHOAN__536C85E5D984BEBD", x => x.Username);
                    table.ForeignKey(
                        name: "FK_TK_NV",
                        column: x => x.MaNhanVien,
                        principalTable: "NHANVIEN",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "KHAUTRULUONG",
                columns: table => new
                {
                    MaKhauTruLuong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaLuong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaKhauTru = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    KhoanKhauTru = table.Column<int>(type: "int", nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KHAUTRUL__E88839032202BA57", x => x.MaKhauTruLuong);
                    table.ForeignKey(
                        name: "FK_KTL_KT",
                        column: x => x.MaKhauTru,
                        principalTable: "KHAUTRU",
                        principalColumn: "MaKhauTru");
                    table.ForeignKey(
                        name: "FK_KTL_L",
                        column: x => x.MaLuong,
                        principalTable: "LUONG",
                        principalColumn: "MaLuong");
                });

            migrationBuilder.CreateTable(
                name: "LUONGPHUCAP",
                columns: table => new
                {
                    MaLuongPhuCap = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaLuong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaPhuCap = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    SoTienPhuCap = table.Column<int>(type: "int", nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LUONGPHU__735D9D6764EE17C8", x => x.MaLuongPhuCap);
                    table.ForeignKey(
                        name: "FK_LPC_L",
                        column: x => x.MaLuong,
                        principalTable: "LUONG",
                        principalColumn: "MaLuong");
                    table.ForeignKey(
                        name: "FK_LPC_PC",
                        column: x => x.MaPhuCap,
                        principalTable: "PHUCAP",
                        principalColumn: "MaPhuCap");
                });

            migrationBuilder.CreateTable(
                name: "PHAT",
                columns: table => new
                {
                    MaPhat = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaLuong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    NgayPhat = table.Column<DateTime>(type: "datetime", nullable: false),
                    SoTienPhat = table.Column<int>(type: "int", nullable: false),
                    LyDoPhat = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PHAT__4AC072E20B5D2876", x => x.MaPhat);
                    table.ForeignKey(
                        name: "FK_P_L",
                        column: x => x.MaLuong,
                        principalTable: "LUONG",
                        principalColumn: "MaLuong");
                });

            migrationBuilder.CreateTable(
                name: "THUONG",
                columns: table => new
                {
                    MaThuong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    MaLuong = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    NgayThuong = table.Column<DateTime>(type: "datetime", nullable: false),
                    SoTienThuong = table.Column<int>(type: "int", nullable: false),
                    LyDoThuong = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__THUONG__5353CF4328103F33", x => x.MaThuong);
                    table.ForeignKey(
                        name: "FK_T_L",
                        column: x => x.MaLuong,
                        principalTable: "LUONG",
                        principalColumn: "MaLuong");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BAOCAO_MaNhanVien",
                table: "BAOCAO",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_CHAMCONG_MaNhanVien",
                table: "CHAMCONG",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_CHUCVU_MaPhongBan",
                table: "CHUCVU",
                column: "MaPhongBan");

            migrationBuilder.CreateIndex(
                name: "IX_HOPDONG_MaNhanVien",
                table: "HOPDONG",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_KHAUTRULUONG_MaKhauTru",
                table: "KHAUTRULUONG",
                column: "MaKhauTru");

            migrationBuilder.CreateIndex(
                name: "IX_KHAUTRULUONG_MaLuong",
                table: "KHAUTRULUONG",
                column: "MaLuong");

            migrationBuilder.CreateIndex(
                name: "IX_LUONG_MaNhanVien",
                table: "LUONG",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_LUONGPHUCAP_MaLuong",
                table: "LUONGPHUCAP",
                column: "MaLuong");

            migrationBuilder.CreateIndex(
                name: "IX_LUONGPHUCAP_MaPhuCap",
                table: "LUONGPHUCAP",
                column: "MaPhuCap");

            migrationBuilder.CreateIndex(
                name: "IX_NGHIPHEP_MaNhanVien",
                table: "NGHIPHEP",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_NHANVIEN_MaChucVu",
                table: "NHANVIEN",
                column: "MaChucVu");

            migrationBuilder.CreateIndex(
                name: "IX_PHANCONG_MaCongViec",
                table: "PHANCONG",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_PHANCONG_MaNhanVien",
                table: "PHANCONG",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_PHAT_MaLuong",
                table: "PHAT",
                column: "MaLuong");

            migrationBuilder.CreateIndex(
                name: "UQ__TAIKHOAN__77B2CA4624BF6A51",
                table: "TAIKHOAN",
                column: "MaNhanVien",
                unique: true,
                filter: "[MaNhanVien] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_THUONG_MaLuong",
                table: "THUONG",
                column: "MaLuong");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BAOCAO");

            migrationBuilder.DropTable(
                name: "CHAMCONG");

            migrationBuilder.DropTable(
                name: "HOPDONG");

            migrationBuilder.DropTable(
                name: "KHAUTRULUONG");

            migrationBuilder.DropTable(
                name: "LUONGPHUCAP");

            migrationBuilder.DropTable(
                name: "NGHIPHEP");

            migrationBuilder.DropTable(
                name: "PHANCONG");

            migrationBuilder.DropTable(
                name: "PHAT");

            migrationBuilder.DropTable(
                name: "TAIKHOAN");

            migrationBuilder.DropTable(
                name: "THUONG");

            migrationBuilder.DropTable(
                name: "KHAUTRU");

            migrationBuilder.DropTable(
                name: "PHUCAP");

            migrationBuilder.DropTable(
                name: "CONGVIEC");

            migrationBuilder.DropTable(
                name: "LUONG");

            migrationBuilder.DropTable(
                name: "NHANVIEN");

            migrationBuilder.DropTable(
                name: "CHUCVU");

            migrationBuilder.DropTable(
                name: "PHONGBAN");
        }
    }
}
