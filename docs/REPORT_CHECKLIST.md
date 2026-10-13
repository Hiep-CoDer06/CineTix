# Checklist báo cáo bài tập lớn

## 1. Tổng quan

- Mô tả hệ thống CineTix và nghiệp vụ đặt vé.
- Phân tích hai lớp người dùng: Customer và Admin.
- Nêu các chức năng chính và giới hạn của phiên bản hiện tại.

## 2. Thiết kế

- Vẽ Use Case Diagram.
- Vẽ ERD: Genre, Movie, Cinema, Showtime, Seat, Booking, BookingSeat và các bảng Identity.
- Ghi rõ khóa chính, khóa ngoại, quan hệ một-nhiều và các unique index.
- Vẽ Sitemap và Wireframe; đối chiếu với giao diện đã triển khai.

## 3. Triển khai

- Trình bày MVC, Entity Framework Core, DbContext, Migration.
- Chụp ảnh CRUD của Admin và luồng đặt vé của Customer.
- Trình bày phân quyền Identity, validation, anti-forgery token.
- Mô tả REST API, AJAX, Session/Cookie, Pagination và Việt/Anh.

## 4. Kiểm thử

- Dùng các test case tại `TEST_CASES.md`.
- Với mỗi case: mục tiêu, dữ liệu đầu vào, kết quả dự kiến, kết quả thực tế, Pass/Fail và cách xử lý lỗi nếu có.

## 5. Kết luận

- Kết quả đạt được.
- Kiến thức rút ra: MVC, EF Core, Identity, AJAX, REST API.
- Hướng phát triển: thanh toán online, email vé, mã QR thật, concurrency token chống đặt trùng ghế, dashboard doanh thu.
