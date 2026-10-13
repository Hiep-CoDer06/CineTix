# Danh mục test case CineTix

| Mã | Mục tiêu | Dữ liệu/bước thực hiện | Kết quả dự kiến |
|---|---|---|---|
| TC01 | Đăng ký Customer | Nhập email hợp lệ, mật khẩu đạt chính sách, xác nhận đúng mật khẩu | Tài khoản được tạo và có role Customer. |
| TC02 | Chặn đăng nhập sai | Nhập sai mật khẩu 5 lần liên tiếp | Identity khóa tạm thời theo cấu hình Lockout. |
| TC03 | Phân quyền Admin | Đăng nhập Customer rồi truy cập `/Movies/Create` | Server chuyển đến Access Denied hoặc từ chối truy cập. |
| TC04 | Lọc và phân trang phim | Tạo ít nhất 9 phim, lọc theo thể loại/trạng thái | Kết quả đúng bộ lọc, mỗi trang tối đa 8 phim, link trang vẫn giữ bộ lọc. |
| TC05 | AJAX tải ghế | Đăng nhập, vào Đặt vé, đổi suất chiếu | Danh sách ghế được cập nhật không tải lại toàn bộ trang. |
| TC06 | Đặt vé hợp lệ | Chọn suất chiếu, ít nhất một ghế trống, nhập thông tin hợp lệ | Tạo Booking và BookingSeat; ghế chuyển sang đã đặt. |
| TC07 | Chặn ghế không hợp lệ | Gửi form với ghế đã đặt hoặc thuộc suất khác | Không tạo vé, hiển thị thông báo lỗi. |
| TC08 | Hủy vé | Customer mở vé của chính mình và hủy | Booking bị xóa, các ghế của vé được chuyển về trạng thái trống. |
| TC09 | Chặn truy cập vé người khác | Customer A truy cập URL Details của Booking thuộc Customer B | Nhận NotFound/không xem được dữ liệu. |
| TC10 | Chuyển ngôn ngữ | Bấm VI/EN ở thanh điều hướng | Cookie văn hóa được lưu; các nhãn thanh điều hướng đổi theo ngôn ngữ. |

Khi viết báo cáo, chụp ảnh đầu vào, kết quả thực tế và ghi rõ Pass/Fail cho từng test case.
