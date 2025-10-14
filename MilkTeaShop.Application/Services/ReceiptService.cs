using MilkTeaShop.Domain.Entities;

namespace MilkTeaShop.Application.Services;

public interface IReceiptService
{
    string GenerateReceipt(Order order, string customerNote = "");
    void PrintReceipt(string receipt);
}

public class ReceiptService : IReceiptService
{
    public string GenerateReceipt(Order order, string customerNote = "")
    {
        var receipt = "";
        
        // Header
        receipt += "════════════════════════════════════════\n";
        receipt += "         🧋 WANGHOC MILK TEA 🧋         \n";
        receipt += "════════════════════════════════════════\n";
        receipt += $"Mã đơn hàng: {order.Id[..8]}\n";
        receipt += $"Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
        receipt += "════════════════════════════════════════\n";
        receipt += "CHI TIẾT ĐƠN HÀNG:\n";
        receipt += "════════════════════════════════════════\n";

        // Items
        foreach (var item in order.Items)
        {
            receipt += $"{item.Description}\n";
            receipt += $"Size: {item.Size} | Số lượng: {item.Quantity}\n";
            
            // Display customizations if any
            if (!string.IsNullOrEmpty(item.SugarLevel) && item.SugarLevel != "100%")
                receipt += $"Đường: {item.SugarLevel} | ";
            if (!string.IsNullOrEmpty(item.IceLevel) && item.IceLevel != "100%")
                receipt += $"Đá: {item.IceLevel}";
            if ((!string.IsNullOrEmpty(item.SugarLevel) && item.SugarLevel != "100%") || 
                (!string.IsNullOrEmpty(item.IceLevel) && item.IceLevel != "100%"))
                receipt += "\n";
            
            // Display toppings if any
            if (item.Toppings != null && item.Toppings.Any())
            {
                receipt += $"Topping: {string.Join(", ", item.Toppings)}\n";
            }
            
            receipt += $"Đơn giá: {item.GetPrice():N0}đ\n";
            receipt += $"Thành tiền: {item.LineTotal:N0}đ\n";
            receipt += "----------------------------------------\n";
        }

        // Totals
        receipt += "════════════════════════════════════════\n";
        receipt += $"Tạm tính: {order.Subtotal:N0}đ\n";
        if (order.Discount > 0)
            receipt += $"Giảm giá: -{order.Discount:N0}đ\n";
        receipt += $"TỔNG CỘNG: {order.Total:N0}đ\n";
        receipt += "════════════════════════════════════════\n";

        // Customer note
        if (!string.IsNullOrEmpty(customerNote))
        {
            receipt += $"Ghi chú: {customerNote}\n";
            receipt += "════════════════════════════════════════\n";
        }

        // Footer
        receipt += "🌟 Cảm ơn quý khách đã sử dụng dịch vụ! 🌟\n";
        receipt += "      Hẹn gặp lại tại WangHoc Milk Tea!    \n";
        receipt += "════════════════════════════════════════";

        return receipt;
    }

    public void PrintReceipt(string receipt)
    {
        // Trong thực tế sẽ gửi đến máy in
        Console.WriteLine("PRINTING RECEIPT:");
        Console.WriteLine(receipt);
    }
}