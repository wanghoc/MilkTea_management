using System.Collections.ObjectModel;
using System.Windows;
using MilkTeaShop.Domain.Entities;
using MilkTeaShop.Domain.ValueObjects;
using MilkTeaShop.Application.Services;
using MilkTeaShop.Infrastructure.Services;
using MilkTeaShop.Domain.Patterns.Decorator;
using MilkTeaShop.Domain.Data;
using MilkTeaShop.Domain.Interfaces;

namespace MilkTeaShop.Presentation.ViewModels;

public class MainPOSViewModel : BaseViewModel
{
    private readonly IMenuService _menuService;
    private readonly IEfReceiptService _receiptService;
    private readonly Order _currentOrder = new();

    // UI Collections
    public ObservableCollection<MenuItem> MilkTeaItems { get; } = new();
    public ObservableCollection<MenuItem> ToppingItems { get; } = new();
    public ObservableCollection<OrderItem> CartItems { get; } = new();
    public ObservableCollection<MenuItem> SelectedToppings { get; } = new();

    // UI Properties
    private int _selectedTabIndex = 0;
    private MenuItem? _selectedMilkTea;
    private string _quantity = "1";
    private string _selectedSize = "Medium";
    private string _selectedSugarLevel = "100%";
    private string _selectedIceLevel = "100%";

    // Commands
    public RelayCommand SelectMilkTeaCommand { get; private set; }
    public RelayCommand SelectToppingCommand { get; private set; }
    public RelayCommand RemoveToppingCommand { get; private set; }
    public RelayCommand AddToCartCommand { get; private set; }
    public RelayCommand RemoveFromCartCommand { get; private set; }
    public RelayCommand PaymentCommand { get; private set; }
    public RelayCommand NewOrderCommand { get; private set; }
    public RelayCommand AddNoteCommand { get; private set; }
    public RelayCommand OpenSettingsCommand { get; private set; }

    public MainPOSViewModel()
    {
        try
        {
            _menuService = new EfMenuService(); // Sử dụng SQLite database
            _receiptService = new EfReceiptService(); // Sử dụng SQLite database

            InitializeCommands();
            LoadMenuItems();
            NewOrder();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khởi tạo ứng dụng: {ex.Message}", "Lỗi nghiêm trọng", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Properties for UI binding
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            _selectedTabIndex = value;
            OnPropertyChanged();
        }
    }

    public MenuItem? SelectedMilkTea
    {
        get => _selectedMilkTea;
        set
        {
            _selectedMilkTea = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PreviewPriceText));
            AddToCartCommand?.RaiseCanExecuteChanged();
        }
    }

    public string Quantity
    {
        get => _quantity;
        set
        {
            if (int.TryParse(value, out int qty) && qty > 0)
            {
                _quantity = qty.ToString();
                OnPropertyChanged();
                OnPropertyChanged(nameof(PreviewPriceText));
                AddToCartCommand?.RaiseCanExecuteChanged();
            }
        }
    }

    public string SelectedSize
    {
        get => _selectedSize;
        set
        {
            _selectedSize = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PreviewPriceText));
        }
    }

    public string SelectedSugarLevel
    {
        get => _selectedSugarLevel;
        set
        {
            _selectedSugarLevel = value;
            OnPropertyChanged();
        }
    }

    public string SelectedIceLevel
    {
        get => _selectedIceLevel;
        set
        {
            _selectedIceLevel = value;
            OnPropertyChanged();
        }
    }

    // Computed Properties
    public List<string> Sizes => new() { "Small", "Medium", "Large" };
    public List<string> SugarLevels => new() { "0%", "25%", "50%", "75%", "100%" };
    public List<string> IceLevels => new() { "0%", "25%", "50%", "75%", "100%" };

    public string OrderId => $"#{_currentOrder.Id[..8]}";
    public string SubtotalText => $"Tạm tính: {_currentOrder.Subtotal:N0}đ";
    public string DiscountText => _currentOrder.Discount > 0 ? $"Giảm giá: -{_currentOrder.Discount:N0}đ" : "";
    public string TotalText => $"TỔNG CỘNG: {_currentOrder.Total:N0}đ";
    
    public string SelectedToppingsText
    {
        get
        {
            if (!SelectedToppings.Any()) return "Chưa chọn topping";
            return $"Đã chọn: {string.Join(", ", SelectedToppings.Select(t => t.Name))}";
        }
    }

    public string PreviewPriceText
    {
        get
        {
            if (SelectedMilkTea == null) return "";
            
            // Create base drink
            IPriceable drink = new BaseDrink(SelectedMilkTea.Name, SelectedMilkTea.BasePrice);
            decimal baseDrinkPrice = SelectedMilkTea.BasePrice;
            decimal toppingsPrice = 0;
            
            // Apply toppings using decorator pattern to get accurate pricing
            var toppingDetails = new List<string>();
            foreach (var topping in SelectedToppings)
            {
                drink = CreateToppingDecorator(topping.Name, drink);
                toppingsPrice += topping.BasePrice;
                toppingDetails.Add($"{topping.Name} (+{topping.BasePrice:N0}đ)");
            }
            
            var totalPrice = drink.GetPrice();
            
            // Apply size modifier
            if (Enum.TryParse<SizeOption>(SelectedSize, out var size))
            {
                totalPrice = StaticMenuData.CalculatePriceBySize(totalPrice, size);
            }

            if (int.TryParse(Quantity, out int qty))
            {
                totalPrice *= qty;
            }

            // Create detailed price breakdown
            var breakdown = new List<string>();
            breakdown.Add($"{SelectedMilkTea.Name}: {baseDrinkPrice:N0}đ");
            
            if (toppingDetails.Any())
            {
                breakdown.Add($"Topping: {string.Join(", ", toppingDetails)}");
            }
            
            breakdown.Add($"Size {SelectedSize}: {GetSizeMultiplierText()}");
            
            if (qty > 1)
            {
                breakdown.Add($"Số lượng: x{qty}");
            }
            
            var priceText = $"💰 Tổng giá: {totalPrice:N0}đ";
            var detailText = string.Join(" | ", breakdown);
            
            return $"{priceText}\n📋 {detailText}";
        }
    }
    
    private string GetSizeMultiplierText()
    {
        return SelectedSize switch
        {
            "Small" => "-15%",
            "Medium" => "Chuẩn",
            "Large" => "+15%",
            _ => "Chuẩn"
        };
    }

    private void InitializeCommands()
    {
        SelectMilkTeaCommand = new RelayCommand(SelectMilkTea);
        SelectToppingCommand = new RelayCommand(SelectTopping);
        RemoveToppingCommand = new RelayCommand(RemoveTopping);
        AddToCartCommand = new RelayCommand(AddToCart, CanAddToCart);
        RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
        PaymentCommand = new RelayCommand(ProcessPayment, CanProcessPayment);
        NewOrderCommand = new RelayCommand(NewOrder);
        AddNoteCommand = new RelayCommand(AddNote);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
    }

    private void LoadMenuItems()
    {
        try
        {
            MilkTeaItems.Clear();
            ToppingItems.Clear();

            var milkTeaItems = _menuService?.GetMilkTeaItems() ?? new List<MenuItem>();
            var toppingItems = _menuService?.GetToppingItems() ?? new List<MenuItem>();

            foreach (var item in milkTeaItems)
            {
                MilkTeaItems.Add(item);
            }

            foreach (var item in toppingItems)
            {
                ToppingItems.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách món: {ex.Message}", "Lỗi", 
                           MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void SelectMilkTea(object? parameter)
    {
        if (parameter is MenuItem milkTea)
        {
            SelectedMilkTea = milkTea;
            SelectedToppings.Clear();
            OnPropertyChanged(nameof(SelectedToppingsText));
            OnPropertyChanged(nameof(PreviewPriceText));
        }
    }

    private void SelectTopping(object? parameter)
    {
        if (parameter is MenuItem topping && SelectedMilkTea != null)
        {
            if (!SelectedToppings.Contains(topping))
            {
                SelectedToppings.Add(topping);
                OnPropertyChanged(nameof(SelectedToppingsText));
                OnPropertyChanged(nameof(PreviewPriceText));
            }
        }
    }

    private void RemoveTopping(object? parameter)
    {
        if (parameter is MenuItem topping)
        {
            SelectedToppings.Remove(topping);
            OnPropertyChanged(nameof(SelectedToppingsText));
            OnPropertyChanged(nameof(PreviewPriceText));
        }
    }

    private bool CanAddToCart(object? parameter)
    {
        return SelectedMilkTea != null && int.TryParse(Quantity, out int qty) && qty > 0;
    }

    private void AddToCart(object? parameter)
    {
        if (SelectedMilkTea == null || !int.TryParse(Quantity, out int qty) || qty <= 0)
            return;

        try
        {
            // Create base drink
            IPriceable drink = new BaseDrink(SelectedMilkTea.Name, SelectedMilkTea.BasePrice);

            // Apply toppings using decorator pattern
            foreach (var topping in SelectedToppings)
            {
                drink = CreateToppingDecorator(topping.Name, drink);
            }

            // Create order item
            var orderItem = new OrderItem(drink)
            {
                Size = Enum.TryParse<SizeOption>(SelectedSize, out var size) ? size : SizeOption.Medium,
                Quantity = qty,
                SugarLevel = SelectedSugarLevel,
                IceLevel = SelectedIceLevel,
                Toppings = SelectedToppings.Select(t => t.Name).ToList()
            };

            // Add to order and UI
            _currentOrder.AddItem(orderItem);
            CartItems.Add(orderItem);

            // Clear selection
            SelectedMilkTea = null;
            SelectedToppings.Clear();
            Quantity = "1";
            SelectedSize = "Medium";
            SelectedSugarLevel = "100%";
            SelectedIceLevel = "100%";

            OnPropertyChanged(nameof(SelectedToppingsText));
            OnPropertyChanged(nameof(PreviewPriceText));
            UpdateOrderSummary();

            MessageBox.Show("Đã thêm vào giỏ hàng!", "Thông báo", 
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi thêm vào giỏ hàng: {ex.Message}", "Lỗi", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private IPriceable CreateToppingDecorator(string toppingName, IPriceable baseDrink)
    {
        // 🎯 NEW: Tìm topping trong database trước để lấy giá chính xác
        var toppingFromDb = _menuService?.GetToppingItems()
            ?.FirstOrDefault(t => t.Name == toppingName);
        
        // Nếu không tìm thấy, fallback sang StaticMenuData
        if (toppingFromDb == null)
        {
            toppingFromDb = StaticMenuData.ToppingItems
                .FirstOrDefault(t => t.Name == toppingName);
        }
        
        // Try specific decorators first (for backward compatibility and performance)
        var specificDecorator = toppingName switch
        {
            // Pearl toppings
            "Trân châu đen" => new BlackPearlTopping(baseDrink),
            "Trân châu trắng" => new WhitePearlTopping(baseDrink),
            "Trân châu hoàng kim" => new GoldenPearlTopping(baseDrink),
            "Trân châu sương mai" => new CrystalPearlTopping(baseDrink),
            "Trân châu đường đen" => new BrownSugarPearlTopping(baseDrink),
            
            // Jelly toppings
            "Thạch cà phê" => new CoffeeJellyTopping(baseDrink),
            "Thạch dừa" => new CoconutJellyTopping(baseDrink),
            "Thạch trái cây" => new FruitJellyTopping(baseDrink),
            "Thạch phô mai" => new CheeseJellyTopping(baseDrink),
            "Thạch rau câu" => new AgarJellyTopping(baseDrink),
            "Jelly rainbow" => new ColorfulJellyTopping(baseDrink),
            
            // Cream and dessert toppings
            "Kem cheese" => new CreamCheeseTopping(baseDrink),
            "Pudding" => new PuddingTopping(baseDrink),
            "Flan" => new FlanTopping(baseDrink),
            "Bánh tráng nướng" => new RicePaperTopping(baseDrink),
            
            // Bean and seed toppings
            "Đậu đỏ" => new RedBeanTopping(baseDrink),
            "Đậu xanh" => new GreenBeanTopping(baseDrink),
            "Hạt chia" => new ChiaSeedTopping(baseDrink),
            "Hạt sen" => new LotusSeedTopping(baseDrink),
            "Hạt điều" => new CashewTopping(baseDrink),
            
            // Fruit and herb toppings
            "Trái cây tươi" => new FreshFruitTopping(baseDrink),
            "Nha đam" => new AloeVeraTopping(baseDrink),
            "Sương sáo" => new GrassJellyTopping(baseDrink),
            "Khoai môn tím" => new PurpleTaroTopping(baseDrink),
            
            // Legacy support for old names
            "Jelly" => new ColorfulJellyTopping(baseDrink),
            
            _ => null as IPriceable // Return null if not found
        };
        
        // If specific decorator found, use it
        if (specificDecorator != null)
        {
            return specificDecorator;
        }
        
        // 🎯 NEW: Use DynamicTopping for any topping from database (including new ones)
        if (toppingFromDb != null)
        {
            Console.WriteLine($"✅ Using DynamicTopping for '{toppingName}' with price {toppingFromDb.BasePrice:N0}đ");
            return new DynamicTopping(baseDrink, toppingName, toppingFromDb.BasePrice);
        }
        
        // ⚠️ Fallback: Nếu không tìm thấy topping, log warning và return baseDrink
        Console.WriteLine($"⚠️ WARNING: Topping '{toppingName}' not found in database or decorators!");
        return baseDrink;
    }

    private void RemoveFromCart(object? parameter)
    {
        if (parameter is OrderItem item)
        {
            _currentOrder.RemoveItem(item.Id);
            CartItems.Remove(item);
            UpdateOrderSummary();
        }
    }

    private bool CanProcessPayment(object? parameter)
    {
        return CartItems.Any();
    }

    private void ProcessPayment(object? parameter)
    {
        if (!CartItems.Any()) return;

        try
        {
            _currentOrder.Checkout();
            
            var receipt = _receiptService.GenerateReceipt(_currentOrder, "");
            
            var receiptWindow = new ReceiptWindow();
            receiptWindow.SetReceiptContent(receipt);
            receiptWindow.ShowDialog();
            
            NewOrder();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi thanh toán: {ex.Message}", "Lỗi", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void NewOrder(object? parameter = null)
    {
        CartItems.Clear();
        _currentOrder.Items.Clear();
        SelectedMilkTea = null;
        SelectedToppings.Clear();
        Quantity = "1";
        SelectedSize = "Medium";
        SelectedSugarLevel = "100%";
        SelectedIceLevel = "100%";
        
        OnPropertyChanged(nameof(SelectedToppingsText));
        OnPropertyChanged(nameof(PreviewPriceText));
        UpdateOrderSummary();
    }

    private void AddNote(object? parameter)
    {
        // Implementation for adding notes
        MessageBox.Show("Chức năng ghi chú đang được phát triển!", "Thông báo", 
                       MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OpenSettings(object? parameter)
    {
        try
        {
            var settingsWindow = new SettingsWindow();
            if (System.Windows.Application.Current?.MainWindow != null)
            {
                settingsWindow.Owner = System.Windows.Application.Current.MainWindow;
            }
            
            if (settingsWindow.ShowDialog() == true)
            {
                LoadMenuItems(); // Refresh menu after settings change
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi mở cài đặt: {ex.Message}", "Lỗi", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateOrderSummary()
    {
        OnPropertyChanged(nameof(OrderId));
        OnPropertyChanged(nameof(SubtotalText));
        OnPropertyChanged(nameof(DiscountText));
        OnPropertyChanged(nameof(TotalText));
        PaymentCommand?.RaiseCanExecuteChanged();
    }
}