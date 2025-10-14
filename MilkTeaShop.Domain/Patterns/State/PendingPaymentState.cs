using MilkTeaShop.Domain.Entities;

namespace MilkTeaShop.Domain.Patterns.State;

public class PendingPaymentState : IOrderState
{
    public string Name => "PendingPayment";

    public void AddItem(Order order, OrderItem item) { /* lock items in pending state if desired */ }
    public void RemoveItem(Order order, string itemId) { /* no-op */ }
    public void Checkout(Order order) => order.SetState(new PaidState());
}
