using MilkTeaShop.Domain.Interfaces;
using MilkTeaShop.Domain.ValueObjects;

namespace MilkTeaShop.Domain.Entities;

public class MenuItem : IPriceable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public MenuCategory Category { get; set; }
    public string ImagePath { get; set; } = string.Empty; // Bạn sẽ thêm hình ảnh vào đây sau
    public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    public decimal GetPrice() => BasePrice;
    public string GetDescription() => Name;
}