namespace MilkTeaShop.Domain.Patterns.Strategy;

public class PercentDiscountStrategy : IPromotionStrategy
{
    private readonly decimal _percent; // 0.2 = 20%
    public string Name => $"Percent {_percent:P0}";

    public PercentDiscountStrategy(decimal percent) => _percent = percent;
    public decimal CalculateDiscount(decimal subtotal) => Math.Max(0, subtotal * _percent);
}
