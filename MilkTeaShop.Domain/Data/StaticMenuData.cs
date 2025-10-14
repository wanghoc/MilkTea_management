using MilkTeaShop.Domain.Entities;
using MilkTeaShop.Domain.ValueObjects;
using System.IO;
using System.Text.Json;

namespace MilkTeaShop.Domain.Data;

public static class StaticMenuData
{
    private static readonly string DataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menu_data.json");
    private static bool _dataLoaded = false;
    
    private static List<MenuItem> _milkTeaItems = new()
    {
        // Các món trà sữa kinh điển
        new MenuItem { Name = "Trà sữa truyền thống", BasePrice = 35000, Category = MenuCategory.MilkTea, Description = "Trà sữa nguyên chất theo công thức truyền thống" },
        new MenuItem { Name = "Trà sữa thai", BasePrice = 40000, Category = MenuCategory.MilkTea, Description = "Trà sữa kiểu Thai với vị đậm đà đặc trưng" },
        new MenuItem { Name = "Trà sữa đường đen", BasePrice = 45000, Category = MenuCategory.MilkTea, Description = "Trà sữa với đường đen Taiwan thơm nồng" },
        new MenuItem { Name = "Trà sữa matcha", BasePrice = 48000, Category = MenuCategory.MilkTea, Description = "Trà sữa matcha Nhật Bản cao cấp" },
        new MenuItem { Name = "Trà sữa khoai môn", BasePrice = 42000, Category = MenuCategory.MilkTea, Description = "Trà sữa khoai môn tím béo ngậy" },
        
        // Các món trà sữa trái cây
        new MenuItem { Name = "Trà sữa dâu", BasePrice = 44000, Category = MenuCategory.MilkTea, Description = "Trà sữa dâu tây tươi ngọt mát" },
        new MenuItem { Name = "Trà sữa xoài", BasePrice = 46000, Category = MenuCategory.MilkTea, Description = "Trà sữa xoài cat chu ngọt thanh" },
        new MenuItem { Name = "Trà sữa đào", BasePrice = 45000, Category = MenuCategory.MilkTea, Description = "Trà sữa đào tiên ngọt dịu" },
        new MenuItem { Name = "Trà sữa việt quất", BasePrice = 50000, Category = MenuCategory.MilkTea, Description = "Trà sữa việt quất tươi giàu vitamin" },
        new MenuItem { Name = "Trà sữa kiwi", BasePrice = 47000, Category = MenuCategory.MilkTea, Description = "Trà sữa kiwi chua ngọt sảng khoái" },
        
        // Các món trà sữa đặc biệt
        new MenuItem { Name = "Trà sữa kem cheese", BasePrice = 55000, Category = MenuCategory.MilkTea, Description = "Trà sữa với lớp kem cheese mặn ngọt hấp dẫn" },
        new MenuItem { Name = "Trà sữa chocolate", BasePrice = 49000, Category = MenuCategory.MilkTea, Description = "Trà sữa chocolate Bỉ thơm nồng" },
        new MenuItem { Name = "Trà sữa caramel", BasePrice = 48000, Category = MenuCategory.MilkTea, Description = "Trà sữa caramel ngọt ngào quyến rũ" },
        new MenuItem { Name = "Trà sữa hokkaido", BasePrice = 52000, Category = MenuCategory.MilkTea, Description = "Trà sữa Hokkaido Nhật Bản cao cấp" },
        new MenuItem { Name = "Trà sữa okinawa", BasePrice = 50000, Category = MenuCategory.MilkTea, Description = "Trà sữa Okinawa với đường nâu đặc trưng" },
        
        // Các món trà sữa mới lạ
        new MenuItem { Name = "Trà sữa lavender", BasePrice = 53000, Category = MenuCategory.MilkTea, Description = "Trà sữa hoa oải hương thơm dịu nhẹ" },
        new MenuItem { Name = "Trà sữa yakult", BasePrice = 46000, Category = MenuCategory.MilkTea, Description = "Trà sữa yakult chua ngọt độc đáo" },
        new MenuItem { Name = "Trà sữa dừa", BasePrice = 44000, Category = MenuCategory.MilkTea, Description = "Trà sữa dừa tươi mát lạnh" },
        new MenuItem { Name = "Trà sữa oolong", BasePrice = 47000, Category = MenuCategory.MilkTea, Description = "Trà sữa oolong Đài Loan thơm nồng" },
        new MenuItem { Name = "Trà sữa bạc hà", BasePrice = 45000, Category = MenuCategory.MilkTea, Description = "Trà sữa bạc hà mát lạnh sảng khoái" }
    };

    private static List<MenuItem> _toppingItems = new()
    {
        // Các loại trân châu
        new MenuItem { Name = "Trân châu đen", BasePrice = 8000, Category = MenuCategory.Topping, Description = "Trân châu đen Taiwan dai ngon đặc trưng" },
        new MenuItem { Name = "Trân châu trắng", BasePrice = 8000, Category = MenuCategory.Topping, Description = "Trân châu trắng mềm mịn trong suốt" },
        new MenuItem { Name = "Trân châu hoàng kim", BasePrice = 12000, Category = MenuCategory.Topping, Description = "Trân châu hoàng kim cao cấp đặc biệt" },
        new MenuItem { Name = "Trân châu sương mai", BasePrice = 10000, Category = MenuCategory.Topping, Description = "Trân châu sương mai trong suốt như ngọc trai" },
        new MenuItem { Name = "Trân châu đường đen", BasePrice = 15000, Category = MenuCategory.Topping, Description = "Trân châu đường đen Taiwan ngọt đậm đà" },
        
        // Các loại thạch
        new MenuItem { Name = "Thạch cà phê", BasePrice = 10000, Category = MenuCategory.Topping, Description = "Thạch cà phê đắng nhẹ thơm lừng" },
        new MenuItem { Name = "Thạch dừa", BasePrice = 9000, Category = MenuCategory.Topping, Description = "Thạch dừa tươi mát thanh nhiệt" },
        new MenuItem { Name = "Thạch trái cây", BasePrice = 11000, Category = MenuCategory.Topping, Description = "Thạch trái cây nhiều màu sắc bắt mắt" },
        new MenuItem { Name = "Thạch phô mai", BasePrice = 13000, Category = MenuCategory.Topping, Description = "Thạch phô mai mềm mịn béo ngậy" },
        new MenuItem { Name = "Thạch rau câu", BasePrice = 8000, Category = MenuCategory.Topping, Description = "Thạch rau câu trong suốt mát lạnh" },
        
        // Các loại kem và bánh
        new MenuItem { Name = "Kem cheese", BasePrice = 18000, Category = MenuCategory.Topping, Description = "Lớp kem cheese mặn ngọt hấp dẫn" },
        new MenuItem { Name = "Pudding", BasePrice = 14000, Category = MenuCategory.Topping, Description = "Pudding caramel mềm mịn thơm ngon" },
        new MenuItem { Name = "Flan", BasePrice = 15000, Category = MenuCategory.Topping, Description = "Bánh flan caramel truyền thống" },
        new MenuItem { Name = "Bánh tráng nướng", BasePrice = 12000, Category = MenuCategory.Topping, Description = "Bánh tráng nướng giòn rụm độc đáo" },
        
        // Các loại đậu và hạt
        new MenuItem { Name = "Đậu đỏ", BasePrice = 9000, Category = MenuCategory.Topping, Description = "Đậu đỏ bùi bùi ngọt thanh" },
        new MenuItem { Name = "Đậu xanh", BasePrice = 9000, Category = MenuCategory.Topping, Description = "Đậu xanh mát lạnh giải nhiệt" },
        new MenuItem { Name = "Hạt chia", BasePrice = 12000, Category = MenuCategory.Topping, Description = "Hạt chia siêu thực phẩm giàu dinh dưỡng" },
        new MenuItem { Name = "Hạt sen", BasePrice = 11000, Category = MenuCategory.Topping, Description = "Hạt sen tươi bùi bùi thơm ngon" },
        new MenuItem { Name = "Hạt điều", BasePrice = 16000, Category = MenuCategory.Topping, Description = "Hạt điều rang giòn thơm béo" },
        
        // Các loại trái cây và thảo mộc
        new MenuItem { Name = "Trái cây tươi", BasePrice = 20000, Category = MenuCategory.Topping, Description = "Trái cây tươi theo mùa đa dạng" },
        new MenuItem { Name = "Nha đam", BasePrice = 10000, Category = MenuCategory.Topping, Description = "Nha đam tươi mát giải nhiệt thanh lọc" },
        new MenuItem { Name = "Sương sáo", BasePrice = 9000, Category = MenuCategory.Topping, Description = "Sương sáo mát lạnh thanh nhiệt truyền thống" },
        new MenuItem { Name = "Khoai môn tím", BasePrice = 12000, Category = MenuCategory.Topping, Description = "Khoai môn tím thái viên béo ngậy" },
        new MenuItem { Name = "Jelly rainbow", BasePrice = 10000, Category = MenuCategory.Topping, Description = "Thạch jelly 7 màu sắc rực rỡ" }
    };

    public static List<MenuItem> MilkTeaItems 
    { 
        get 
        {
            LoadDataFromFile();
            return _milkTeaItems;
        } 
    }

    public static List<MenuItem> ToppingItems 
    { 
        get 
        {
            LoadDataFromFile();
            return _toppingItems;
        } 
    }

    public static List<MenuItem> GetAllItems() => MilkTeaItems.Concat(ToppingItems).ToList();
    
    public static void AddNewItem(MenuItem item)
    {
        LoadDataFromFile(); // Ensure we have latest data
        
        if (item.Category == MenuCategory.MilkTea)
            _milkTeaItems.Add(item);
        else
            _toppingItems.Add(item);
            
        SaveDataToFile();
        _dataLoaded = false; // Force reload next time
    }

    public static bool RemoveItem(MenuItem item)
    {
        LoadDataFromFile();
        
        bool removed = false;
        if (item.Category == MenuCategory.MilkTea)
        {
            removed = _milkTeaItems.Remove(item);
        }
        else
        {
            removed = _toppingItems.Remove(item);
        }
        
        if (removed)
        {
            SaveDataToFile();
            _dataLoaded = false; // Force reload next time
        }
        
        return removed;
    }

    public static void UpdateItem(MenuItem oldItem, MenuItem newItem)
    {
        LoadDataFromFile();
        
        var list = oldItem.Category == MenuCategory.MilkTea ? _milkTeaItems : _toppingItems;
        var index = list.IndexOf(oldItem);
        
        if (index >= 0)
        {
            list[index] = newItem;
            SaveDataToFile();
            _dataLoaded = false; // Force reload next time
        }
    }

    public static void ForceReload()
    {
        _dataLoaded = false;
        LoadDataFromFile();
    }

    // Helper methods để lấy tên các loại trà sữa và topping
    public static List<string> GetMilkTeaNames() => MilkTeaItems.Select(x => x.Name).ToList();
    public static List<string> GetToppingNames() => ToppingItems.Select(x => x.Name).ToList();

    // Method to calculate price based on size (Medium is base price, Small -15%, Large +15%)
    public static decimal CalculatePriceBySize(decimal basePrice, SizeOption size)
    {
        return size switch
        {
            SizeOption.Small => basePrice * 0.85M,  // -15%
            SizeOption.Medium => basePrice,         // Base price
            SizeOption.Large => basePrice * 1.15M,  // +15%
            _ => basePrice
        };
    }

    private static void LoadDataFromFile()
    {
        if (_dataLoaded) return; // Already loaded
        
        try
        {
            if (File.Exists(DataFilePath))
            {
                var json = File.ReadAllText(DataFilePath);
                var data = JsonSerializer.Deserialize<MenuData>(json);
                if (data != null)
                {
                    _milkTeaItems = data.MilkTeaItems ?? _milkTeaItems;
                    _toppingItems = data.ToppingItems ?? _toppingItems;
                }
            }
            _dataLoaded = true;
        }
        catch
        {
            // If loading fails, use default data
            _dataLoaded = true;
        }
    }

    private static void SaveDataToFile()
    {
        try
        {
            var data = new MenuData
            {
                MilkTeaItems = _milkTeaItems,
                ToppingItems = _toppingItems
            };
            
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DataFilePath, json);
        }
        catch
        {
            // Ignore save errors for now
        }
    }

    private class MenuData
    {
        public List<MenuItem> MilkTeaItems { get; set; } = new();
        public List<MenuItem> ToppingItems { get; set; } = new();
    }
}