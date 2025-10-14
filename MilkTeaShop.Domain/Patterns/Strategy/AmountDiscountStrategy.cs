namespace MilkTeaShop.Domain.Patterns.Strategy;

public class AmountDiscountStrategy : IPromotionStrategy
{
    private readonly decimal _amount;
    public string Name => $"Amount {_amount:0,0}";

    public AmountDiscountStrategy(decimal amount) => _amount = amount;
    public decimal CalculateDiscount(decimal subtotal) => Math.Clamp(_amount, 0, subtotal);
}
