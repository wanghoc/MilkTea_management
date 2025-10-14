namespace MilkTeaShop.Infrastructure.Adapters.Printing;

public class ConsoleReceiptPrinter : IReceiptPrinter
{
    public void Print(string content) => System.Console.WriteLine(content);
}
