using MilkTeaShop.Domain.Entities;
using MilkTeaShop.Domain.Patterns.Strategy;

namespace MilkTeaShop.Application.Services;

public class PricingService
{
    public (decimal discount, string strategy) ApplyPromotion(Order order, IPromotionStrategy strategy)
        => (strategy.CalculateDiscount(order.Subtotal), strategy.Name);
}
