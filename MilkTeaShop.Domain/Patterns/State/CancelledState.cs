using MilkTeaShop.Domain.Entities;

namespace MilkTeaShop.Domain.Patterns.State;

public class CancelledState : IOrderState
{
    public string Name => "Cancelled";

    public void AddItem(Order order, OrderItem item) { }
    public void RemoveItem(Order order, string itemId) { }
    public void Checkout(Order order) { }
}
