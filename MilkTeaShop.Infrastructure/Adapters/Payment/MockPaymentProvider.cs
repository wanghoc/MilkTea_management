namespace MilkTeaShop.Infrastructure.Adapters.Payment;

public class MockPaymentProvider : IPaymentProvider
{
    public Task<bool> PayAsync(decimal amount, string method)
        => Task.FromResult(true); // always succeed in demo
}
