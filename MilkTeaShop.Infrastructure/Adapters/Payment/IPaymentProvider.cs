namespace MilkTeaShop.Infrastructure.Adapters.Payment;

public interface IPaymentProvider
{
    Task<bool> PayAsync(decimal amount, string method);
}
