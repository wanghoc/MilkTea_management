using MilkTeaShop.Domain.Entities;

namespace MilkTeaShop.Domain.Patterns.State;

public class PaidState : IOrderState
{
    public string Name => "Paid";

    public void AddItem(Order order, OrderItem item) { /* no changes allowed */ }
    public void RemoveItem(Order order, string itemId) { /* no changes allowed */ }
    public void Checkout(Order order) { /* already paid */ }
}
