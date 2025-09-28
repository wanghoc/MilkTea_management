# Hướng dẫn thêm hình ảnh cho món

## 1. Cấu trúc thư mục
Ứng dụng sẽ tự động tạo thư mục `Images` trong thư mục chứa file thực thi (.exe). Cấu trúc như sau:

```
MilkTeaShop.Presentation/
├── bin/
│   └── Debug/ (hoặc Release/)
│       ├── net8.0-windows/
│       │   ├── MilkTeaShop.Presentation.exe
│       │   └── Images/          ← Thư mục chứa ảnh
│       │       ├── milk-tea-1.jpg
│       │       ├── milk-tea-2.png
│       │       └── topping-1.jpg
```

## 2. Cách thêm ảnh

### Phương pháp 1: Thông qua ứng dụng (Khuyến nghị)
1. Mở ứng dụng và vào **Cài đặt**
2. Nhấn **"➕ Thêm món"** hoặc **"✏️ Sửa"** món có sẵn
3. Nhấn nút **"Chọn ảnh"**
4. Chọn file ảnh từ máy tính
5. Ảnh sẽ được tự động sao chép vào thư mục `Images`

### Phương pháp 2: Sao chép trực tiếp
1. Tạo thư mục `Images` trong thư mục chứa file .exe (nếu chưa có)
2. Sao chép các file ảnh vào thư mục này
3. Khi thêm/sửa món, nhập đường dẫn tương đối: `Images/ten-file.jpg`

## 3. Yêu cầu về ảnh

### Kích thước
- **Khuyến nghị**: 300x300 pixels (tỷ lệ 1:1)
- **Tối thiểu**: 150x150 pixels
- **Tối đa**: 1024x1024 pixels

### Định dạng hỗ trợ
- ✅ JPG/JPEG
- ✅ PNG (khuyến nghị cho ảnh có nền trong suốt)
- ✅ GIF
- ✅ BMP

### Dung lượng
- **Tối đa**: 5MB per file
- **Khuyến nghị**: 100KB - 500KB

### Chất lượng
- Sử dụng ảnh có độ phân giải cao
- Đảm bảo ảnh rõ nét, không bị mờ
- Nền sáng hoặc trong suốt để dễ nhìn
- Tránh ảnh có watermark

## 4. Quy tắc đặt tên file (Khuyến nghị)

```
Trà sữa: milktea-{tên}.jpg
- milktea-original.jpg
- milktea-chocolate.jpg
- milktea-matcha.jpg

Topping: topping-{tên}.jpg
- topping-blackpearl.jpg  
- topping-pudding.jpg
- topping-jelly.jpg
```

## 5. Troubleshooting

### Ảnh không hiển thị
1. Kiểm tra đường dẫn file có đúng không
2. Đảm bảo file ảnh nằm trong thư mục `Images`
3. Kiểm tra định dạng file có được hỗ trợ không
4. Thử khởi động lại ứng dụng

### Lỗi "File không tồn tại"
1. Kiểm tra tên file có chính xác không (phân biệt chữ hoa/thường)
2. Đảm bảo file không bị xóa hoặc di chuyển
3. Sử dụng đường dẫn tương đối: `Images/filename.jpg`

### Performance
- Nếu có nhiều ảnh lớn, app có thể chậm khi khởi động
- Nên optimize ảnh trước khi thêm vào
- Sử dụng công cụ nén ảnh online để giảm dung lượng

## 6. Ví dụ cụ thể

### Thêm ảnh cho "Trà sữa matcha"
1. Chuẩn bị file: `matcha-milktea.jpg` (300x300px, ~200KB)
2. Vào **Cài đặt** > **Thêm món**
3. Nhập thông tin món
4. Nhấn **Chọn ảnh** > Chọn `matcha-milktea.jpg`
5. Nhấn **Lưu**

Kết quả: Ảnh sẽ xuất hiện trên menu chính và trong danh sách quản lý.