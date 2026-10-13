# CineTix

Website đặt vé xem phim được xây dựng bằng ASP.NET Core MVC, Entity Framework Core và SQL Server LocalDB.

## Chức năng chính

- Khách xem phim, lọc và phân trang danh sách phim.
- Khách đăng ký/đăng nhập, chọn suất chiếu, chọn ghế, đặt vé và xem vé của mình.
- Admin quản lý phim, thể loại, rạp, suất chiếu và ghế.
- Phân quyền `Admin`/`Customer` bằng ASP.NET Core Identity.
- REST API trả dữ liệu ghế trống; trang đặt vé sử dụng AJAX để tải ghế mà không tải lại toàn bộ trang.
- Session lưu tạm suất chiếu đang chọn; cookie ghi nhớ suất chiếu gần nhất và duy trì đăng nhập.
- Hỗ trợ chuyển ngôn ngữ cơ bản Việt/Anh trên thanh điều hướng.

## Yêu cầu môi trường

- Visual Studio 2022 hoặc mới hơn, workload **ASP.NET and web development**.
- .NET SDK 8.
- SQL Server LocalDB.

## Cài đặt và chạy

1. Mở file `CineTix.sln` bằng Visual Studio.
2. Kiểm tra chuỗi kết nối `CineTixConnection` trong `appsettings.json`.
3. Mở **Tools → NuGet Package Manager → Package Manager Console** và chọn project `CineTix`.
4. Chạy lệnh:

   ```powershell
   Update-Database
   ```

5. Nhấn `Ctrl + F5` để chạy ứng dụng.

Migration đã được đưa vào source code. Hướng dẫn tạo script SQL idempotent có tại `scripts/Database-Commands.md`.

## Tài khoản và phân quyền

- Tài khoản đăng ký từ website nhận role `Customer`.
- Tài khoản Admin được tạo bởi `IdentitySeedData` dựa trên User Secrets, không đưa mật khẩu vào source code.

Ví dụ cấu hình User Secrets trong Package Manager Console:

```powershell
dotnet user-secrets set "Identity:SeedAdminEmail" "admin@cinetix.local"
dotnet user-secrets set "Identity:SeedAdminPassword" "MatKhauManh!2026"
```

## Kiểm tra REST API

Sau khi đăng nhập, mở URL sau trong cùng trình duyệt:

```text
https://localhost:7195/api/showtimes/{showtimeId}/booking-data
```

API trả JSON gồm thông tin suất chiếu, giá vé và các ghế còn trống. Xem file `CineTix.http` để có request mẫu.

## Tài liệu kèm theo

- `docs/TEST_CASES.md`: danh mục kiểm thử thủ công.
- `docs/REPORT_CHECKLIST.md`: checklist chuẩn bị báo cáo/thuyết trình.
- `scripts/Database-Commands.md`: lệnh tạo/cập nhật database và xuất script SQL.
