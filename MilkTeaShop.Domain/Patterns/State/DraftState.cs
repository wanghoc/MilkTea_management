using MilkTeaShop.Domain.Entities;

namespace MilkTeaShop.Domain.Patterns.State;

public class DraftState : IOrderState
{
    public string Name => "Draft";

    public void AddItem(Order order, OrderItem item) => order.Items.Add(item);
    public void RemoveItem(Order order, string itemId) => order.Items.RemoveAll(i => i.Id == itemId);
    public void Checkout(Order order) => order.SetState(new PendingPaymentState());
}
