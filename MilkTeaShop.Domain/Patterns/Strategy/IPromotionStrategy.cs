namespace MilkTeaShop.Domain.Patterns.Strategy;

public interface IPromotionStrategy
{
    decimal CalculateDiscount(decimal subtotal);
    string Name { get; }
}
