using MilkTeaShop.Domain.Interfaces;

namespace MilkTeaShop.Domain.Patterns.Decorator;

public abstract class DrinkDecorator : IPriceable
{
    protected readonly IPriceable Inner;
    protected DrinkDecorator(IPriceable inner) => Inner = inner;

    public virtual decimal GetPrice() => Inner.GetPrice();
    public virtual string GetDescription() => Inner.GetDescription();
}
