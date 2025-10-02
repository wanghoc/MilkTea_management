using MilkTeaShop.Domain.Interfaces;

namespace MilkTeaShop.Domain.Entities;

// Base drink để bắt đầu decoration chain
public class BaseDrink : IPriceable
{
    public string Name { get; set; }
    public decimal BasePrice { get; set; }

    public BaseDrink()
    {
        Name = "";
        BasePrice = 0m;
    }

    public BaseDrink(string name, decimal basePrice)
    {
        Name = name;
        BasePrice = basePrice;
    }

    public virtual decimal GetPrice() => BasePrice;
    public virtual string GetDescription() => Name;
}