namespace MilkTeaShop.Domain.Entities;

public class Customer
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Tier { get; set; } = "Bronze";
    public int Points { get; set; }
}
