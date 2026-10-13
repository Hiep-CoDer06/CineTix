# Lệnh database

Mở **Tools → NuGet Package Manager → Package Manager Console**, chọn `Default project: CineTix`.

## Tạo database/cập nhật database

```powershell
Update-Database
```

## Khi Model thay đổi

```powershell
Add-Migration TenMoTaThayDoi
Update-Database
```

## Xuất script SQL idempotent để nộp kèm bài

```powershell
Script-Migration -Idempotent -Output scripts\CineTix.Database.sql
```

Script idempotent có thể chạy trên database mới hoặc database đang thiếu một số migration; EF Core chỉ áp dụng migration chưa có trong bảng lịch sử migration.
