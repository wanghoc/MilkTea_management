# Hệ thống Quản lý Trà Sữa

## Tính năng chính đã hoàn thành:

### 1. **Giao diện người dùng**
- **Tab Trà sữa**: Hiển thị 10 loại trà sữa khác nhau với giá cả
- **Tab Topping**: Hiển thị 10 loại topping với giá cả
- **Panel tùy chỉnh**: Cho phép chọn size, đường, đá, số lượng
- **Giỏ hàng**: Hiển thị danh sách món đã chọn với chi tiết
- **Thanh toán**: Hiển thị tạm tính, giảm giá, tổng cộng

### 2. **Design Patterns được sử dụng**

#### **Decorator Pattern** ⭐
- **BaseDrink**: Cơ sở để bắt đầu decoration chain
- **MilkTeaDecorators**: 10 loại trà sữa (Original, Chocolate, Strawberry, Taro, Matcha, Brown Sugar, Cream Cheese, Cocoa, Hokkaido, Okinawa)
- **ToppingDecorators**: 10 loại topping (Black Pearl, White Pearl, Golden Pearl, Pudding, Coffee Jelly, Coconut Jelly, Cream Cheese, Red Bean, Chia Seed, Colorful Jelly)

#### **Factory Pattern**
- **DrinkFactory**: Tạo trà sữa với topping sử dụng decorator pattern

#### **State Pattern**
- **IOrderState**: Interface cho trạng thái đơn hàng
- **DraftState, PendingPaymentState, PaidState, CancelledState**

#### **Strategy Pattern**
- **IPromotionStrategy**: Interface cho các chiến lược giảm giá
- **PercentDiscountStrategy, AmountDiscountStrategy**

#### **Builder Pattern**
- **OrderItemBuilder**: Xây dựng OrderItem với các tùy chỉnh

#### **MVVM Pattern**
- **BaseViewModel**: Cơ sở cho tất cả ViewModels
- **MainPOSViewModel**: ViewModel chính cho giao diện POS
- **RelayCommand**: Command pattern cho WPF

### 3. **Chức năng nghiệp vụ**

#### **Đặt hàng**
1. Chọn trà sữa từ tab "TRÀ SỮA"
2. Chuyển sang tab "TOPPING" để chọn topping (có thể chọn nhiều)
3. Tùy chỉnh: Size (Small/Medium/Large), Đường (0%-100%), Đá (0%-100%)
4. Chọn số lượng
5. Nhấn "THÊM VÀO GIỎ"

#### **Quản lý giỏ hàng**
- Xem danh sách món đã chọn
- Xóa món khỏi giỏ hàng
- Xem tổng tiền tự động cập nhật

#### **Thanh toán**
- Thêm ghi chú cho đơn hàng
- Thanh toán và in hóa đơn
- Tạo đơn hàng mới

#### **Quản lý menu**
- Cài đặt để thêm món mới
- Phân loại món (Trà sữa/Topping)
- Upload hình ảnh (placeholder)

### 4. **Cấu trúc dự án**

```
MilkTeaShop/
├── Domain/                 # Domain Layer
│   ├── Entities/           # Order, OrderItem, MenuItem, etc.
│   ├── ValueObjects/       # SizeOption, MenuCategory
│   ├── Patterns/           # Design Patterns implementation
│   │   ├── Decorator/      # Drink decorators
│   │   ├── State/          # Order states  
│   │   ├── Strategy/       # Promotion strategies
│   │   └── Builder/        # OrderItem builder
│   ├── Factories/          # DrinkFactory
│   └── Data/              # Static menu data
├── Application/            # Application Layer
│   └── Services/          # MenuService, ReceiptService
├── Infrastructure/         # Infrastructure Layer
│   └── Adapters/          # Payment, Printing adapters
└── Presentation/          # Presentation Layer (WPF)
    ├── ViewModels/        # MVVM ViewModels
    ├── Converters/        # XAML Converters
    └── Windows/           # UI Windows
```

### 5. **Cách sử dụng**

1. **Khởi chạy**: `dotnet run --project MilkTeaShop.Presentation`
2. **Thêm món**: Chọn trà sữa → Chọn topping → Tùy chỉnh → Thêm vào giỏ
3. **Thanh toán**: Nhấn "THANH TOÁN" → Xem hóa đơn → In hóa đơn
4. **Thêm món mới**: Nhấn "Cài đặt" → Điền thông tin → "Thêm món"

### 6. **Điểm đặc biệt của Decorator Pattern**

Thay vì tạo từng class riêng cho mỗi combination (VD: MilkTeaWithPearl, MilkTeaWithCheese, etc.), hệ thống sử dụng Decorator Pattern để:

- **Linh hoạt**: Có thể tạo vô số combination mà không cần tạo class mới
- **Mở rộng dễ dàng**: Thêm topping mới chỉ cần tạo 1 decorator
- **Tính giá động**: Giá được tính tự động qua chain of decorators
- **Quản lý tốt**: Code sạch, dễ maintain

**Ví dụ tạo đồ uống**:
```csharp
// Tạo trà sữa matcha với trân châu đen và kem cheese
var drink = DrinkFactory.CreateDrinkWithToppings(
    "Trà sữa matcha", 
    new[] { "Trân châu đen", "Kem cheese" }
);
// Kết quả: BaseDrink -> MatchaMilkTeaDecorator -> BlackPearlTopping -> CreamCheeseTopping
// Giá: 0 + 48000 + 8000 + 15000 = 71000đ
```

### 7. **Hình ảnh**
**Lưu ý**: Các hình ảnh sẽ được thêm vào thư mục `Images/` trong project Presentation. Hiện tại đang để placeholder để bạn có thể thêm hình ảnh sau.

**Đường dẫn cần thêm hình**:
- `MilkTeaShop.Presentation/Images/MilkTea/` - Hình các loại trà sữa
- `MilkTeaShop.Presentation/Images/Toppings/` - Hình các loại topping

Hệ thống đã hoàn thành với đầy đủ tính năng theo yêu cầu và sử dụng Design Patterns một cách hiệu quả!