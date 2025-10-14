using MilkTeaShop.Domain.Entities;

namespace MilkTeaShop.Domain.Patterns.State;

public interface IOrderState
{
    string Name { get; }
    void AddItem(Order order, OrderItem item);
    void RemoveItem(Order order, string itemId);
    void Checkout(Order order);
}
