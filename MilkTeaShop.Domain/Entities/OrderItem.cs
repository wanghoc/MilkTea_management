using MilkTeaShop.Domain.Interfaces;
using MilkTeaShop.Domain.ValueObjects;
using MilkTeaShop.Domain.Data;

namespace MilkTeaShop.Domain.Entities;

public class OrderItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public IPriceable Drink { get; set; }
    public SizeOption Size { get; set; } = SizeOption.Medium;
    public int Quantity { get; set; } = 1;
    public string SugarLevel { get; set; } = "100%"; // 0%, 25%, 50%, 75%, 100%
    public string IceLevel { get; set; } = "100%"; // 0%, 25%, 50%, 75%, 100%
    public List<string> Toppings { get; set; } = new();
    public string Description => Drink.GetDescription();
    
    // Property for displaying details in the cart
    public string Details 
    {
        get
        {
            var parts = new List<string>
            {
                $"SL: {Quantity}",
                $"Size: {Size}",
                $"Đường: {SugarLevel}",
                $"Đá: {IceLevel}"
            };
            
            if (Toppings.Any())
            {
                // Calculate and display topping prices for transparency
                decimal toppingTotal = 0;
                var toppingPrices = new List<string>();
                foreach (var topping in Toppings)
                {
                    var toppingItem = StaticMenuData.ToppingItems.FirstOrDefault(t => t.Name == topping);
                    if (toppingItem != null)
                    {
                        toppingTotal += toppingItem.BasePrice;
                        toppingPrices.Add($"{topping} (+{toppingItem.BasePrice:N0}đ)");
                    }
                    else
                    {
                        toppingPrices.Add(topping);
                    }
                }
                parts.Add($"Topping: {string.Join(", ", toppingPrices)} = +{toppingTotal:N0}đ");
            }
            
            return string.Join(" | ", parts);
        }
    }

    public OrderItem(IPriceable drink) => Drink = drink;

    public decimal GetPrice() 
    {
        // 🎯 FIX: Chỉ dùng decorator pattern - đây là cách đúng!
        // Decorator đã bao gồm base drink + tất cả toppings
        var drinkPriceWithToppings = Drink.GetPrice();
        
        // Apply size adjustment to the final price (including toppings)
        return StaticMenuData.CalculatePriceBySize(drinkPriceWithToppings, Size);
    }
    
    public decimal LineTotal => GetPrice() * Quantity;
    
    // Additional method to get detailed price breakdown for debugging
    public string GetPriceBreakdown()
    {
        var decoratorPrice = Drink.GetPrice();
        var sizeAdjusted = StaticMenuData.CalculatePriceBySize(decoratorPrice, Size);
        var finalPrice = GetPrice();
        
        return $"Decorator: {decoratorPrice:N0}đ | Size {Size}: {sizeAdjusted:N0}đ | Final: {finalPrice:N0}đ";
    }
    
    public override string ToString()
    {
        var toppingsText = Toppings.Any() ? $" + {string.Join(", ", Toppings)}" : "";
        var customText = $" (Đường: {SugarLevel}, Đá: {IceLevel})";
        return $"{Quantity}x {Description}{toppingsText} ({Size}){customText} - {LineTotal:N0}đ";
    }
}
