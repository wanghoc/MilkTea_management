using MilkTeaShop.Domain.ValueObjects;

namespace MilkTeaShop.Presentation.Models;

public class MenuCategoryDisplayItem
{
    public MenuCategory Category { get; set; }
    public string DisplayName { get; set; } = "";

    public MenuCategoryDisplayItem(MenuCategory category)
    {
        Category = category;
        DisplayName = category switch
        {
            MenuCategory.MilkTea => "Trà sữa",
            MenuCategory.Topping => "Topping",
            _ => category.ToString()
        };
    }

    public override string ToString() => DisplayName;
    
    public override bool Equals(object? obj)
    {
        if (obj is MenuCategoryDisplayItem other)
            return Category == other.Category;
        if (obj is MenuCategory category)
            return Category == category;
        return false;
    }

    public override int GetHashCode() => Category.GetHashCode();
}