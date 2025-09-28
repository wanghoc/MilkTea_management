# Hướng dẫn sử dụng thư mục Images

## 📁 Thư mục Images đã được tạo tại:
```
MilkTeaShop.Presentation\bin\Debug\net8.0-windows\Images\
```

## 🎯 Mục đích:
- Lưu trữ tất cả hình ảnh của món trà sữa và topping
- Tự động sao chép ảnh khi thêm món mới qua ứng dụng
- Tự động xóa ảnh khi xóa món (nếu không có món nào khác sử dụng)

## 📋 Quy tắc đặt tên (khuyến nghị):

### Trà sữa:
- `milktea-original.jpg` → Trà sữa nguyên chất
- `milktea-chocolate.jpg` → Trà sữa chocolate  
- `milktea-strawberry.jpg` → Trà sữa dâu
- `milktea-taro.jpg` → Trà sữa khoai môn
- `milktea-matcha.jpg` → Trà sữa matcha
- `milktea-browsugar.jpg` → Trà sữa đường đen
- `milktea-cheese.jpg` → Trà sữa kem cheese
- `milktea-cocoa.jpg` → Trà sữa socola
- `milktea-hokkaido.jpg` → Trà sữa hokkaido
- `milktea-okinawa.jpg` → Trà sữa okinawa

### Topping:
- `topping-blackpearl.jpg` → Trân châu đen
- `topping-whitepearl.jpg` → Trân châu trắng
- `topping-goldenpearl.jpg` → Trân châu hoàng kim
- `topping-pudding.jpg` → Pudding
- `topping-coffeejelly.jpg` → Thạch cà phê
- `topping-coconutjelly.jpg` → Thạch dừa
- `topping-cheese.jpg` → Kem cheese
- `topping-redbean.jpg` → Đậu đỏ
- `topping-chiaseed.jpg` → Hạt chia
- `topping-jelly.jpg` → Jelly

## 🖼️ Yêu cầu ảnh:
- **Kích thước**: 300x300px (tỷ lệ 1:1)
- **Định dạng**: JPG, PNG, GIF, BMP
- **Dung lượng**: Tối đa 5MB, khuyến nghị 100KB-500KB
- **Chất lượng**: Rõ nét, nền sáng

## 💡 Cách thêm ảnh:

### Phương pháp 1: Qua ứng dụng (Tự động)
1. Mở ứng dụng → Cài đặt → Thêm món
2. Nhấn "Chọn ảnh" → Chọn file từ máy tính
3. Ảnh sẽ tự động được sao chép vào thư mục Images

### Phương pháp 2: Sao chép trực tiếp  
1. Sao chép file ảnh vào thư mục này
2. Khi thêm món, nhập đường dẫn: `Images/ten-file.jpg`

## 🗑️ Tự động dọn dẹp:
- Khi xóa món: Ảnh sẽ tự động bị xóa (nếu không món nào khác dùng)
- Khi sửa món và đổi ảnh: Ảnh cũ sẽ tự động bị xóa

## ⚠️ Lưu ý:
- Đừng xóa thư mục Images này
- Nếu có nhiều món dùng chung 1 ảnh, ảnh sẽ không bị xóa khi xóa 1 món
- Backup thư mục này nếu cần thiết