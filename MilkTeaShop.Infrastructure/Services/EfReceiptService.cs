using MilkTeaShop.Domain.Entities;
using MilkTeaShop.Infrastructure.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MilkTeaShop.Infrastructure.Services;

public interface IEfReceiptService
{
    string GenerateReceipt(Order order, string customerNote = "");
    void PrintReceipt(string receipt);
    void SaveReceipt(Order order, string customerNote, string receiptContent);
    List<Receipt> GetAllReceipts();
    List<Receipt> GetReceiptsByDateRange(DateTime from, DateTime to);
    Receipt? GetReceiptById(string id);
    decimal GetTotalSalesForDate(DateTime date);
    List<(string ItemName, int Quantity, decimal Revenue)> GetTopSellingItems(int count = 10);
}

public class EfReceiptService : IEfReceiptService
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
                receipt += $"Đường: {item.SugarLevel}";
            if (!string.IsNullOrEmpty(item.IceLevel) && item.IceLevel != "100%")
                receipt += $" | Đá: {item.IceLevel}";
            if ((!string.IsNullOrEmpty(item.SugarLevel) && item.SugarLevel != "100%") || 
                (!string.IsNullOrEmpty(item.IceLevel) && item.IceLevel != "100%"))
                receipt += "\n";
            
            // Display toppings with prices if any
            if (item.Toppings != null && item.Toppings.Any())
            {
                receipt += "Topping đã chọn:\n";
                foreach (var toppingName in item.Toppings)
                {
                    var toppingItem = MilkTeaShop.Domain.Data.StaticMenuData.ToppingItems
                        .FirstOrDefault(t => t.Name == toppingName);
                    if (toppingItem != null)
                    {
                        receipt += $"  + {toppingName} (+{toppingItem.BasePrice:N0}đ)\n";
                    }
                    else
                    {
                        receipt += $"  + {toppingName}\n";
                    }
                }
            }
            
            // Đảm bảo hiển thị giá tiền với breakdown
            var unitPrice = item.GetPrice();
            var lineTotal = item.LineTotal;
            receipt += $"Đơn giá: {unitPrice:N0}đ | Thành tiền: {lineTotal:N0}đ\n";
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

        // Save to database
        SaveReceipt(order, customerNote, receipt);

        return receipt;
    }

    public void PrintReceipt(string receipt)
    {
        // Trong thực tế sẽ gửi đến máy in
        Console.WriteLine("PRINTING RECEIPT:");
        Console.WriteLine(receipt);
    }

    public void SaveReceipt(Order order, string customerNote, string receiptContent)
    {
        try
        {
            // VALIDATION: Kiểm tra dữ liệu trước khi lưu
            if (order == null)
            {
                Console.WriteLine("ERROR: Order is null, cannot save receipt");
                return;
            }

            if (order.Total <= 0)
            {
                Console.WriteLine($"ERROR: Order total is {order.Total}, cannot save receipt with 0 or negative total");
                return;
            }

            if (!order.Items.Any())
            {
                Console.WriteLine("ERROR: Order has no items, cannot save empty receipt");
                return;
            }

            using var db = new MilkTeaDbContext();
            
            // Kiểm tra số lượng receipts hiện tại trước khi thêm
            var existingReceiptCount = db.Receipts.Count();
            Console.WriteLine($"Existing receipts in database: {existingReceiptCount}");
            
            // Validate existing receipts không bị corruption
            var corruptedReceipts = db.Receipts.Where(r => r.Total <= 0).Count();
            if (corruptedReceipts > 0)
            {
                Console.WriteLine($"WARNING: Found {corruptedReceipts} corrupted receipts with 0 total");
            }
            
            var purchaseTime = DateTime.Now;
            var receiptId = Guid.NewGuid().ToString();
            
            // VALIDATION: Đảm bảo các giá trị hợp lệ
            var validSubtotal = Math.Max(0, order.Subtotal);
            var validDiscount = Math.Max(0, order.Discount);
            var validTotal = Math.Max(0, order.Total);
            
            Console.WriteLine($"Creating receipt - Subtotal: {validSubtotal}, Discount: {validDiscount}, Total: {validTotal}");
            
            if (validTotal <= 0)
            {
                Console.WriteLine("ERROR: Calculated total is 0 or negative, aborting save");
                return;
            }
            
            var receipt = new Receipt
            {
                Id = receiptId,
                OrderId = order.Id,
                CreatedAt = purchaseTime,
                PurchaseTime = purchaseTime,
                Subtotal = validSubtotal,
                Discount = validDiscount,
                Total = validTotal,
                CustomerNote = customerNote ?? "",
                ReceiptContent = receiptContent ?? "",
                PaymentMethod = "Cash"
            };

            Console.WriteLine($"Creating receipt with ID: {receiptId}");
            Console.WriteLine($"Receipt details - Total: {receipt.Total:F2}, Items count: {order.Items.Count}");

            // Tạo receipt items với validation
            foreach (var orderItem in order.Items)
            {
                var itemPrice = orderItem.GetPrice();
                var itemTotal = orderItem.LineTotal;
                
                // VALIDATION: Kiểm tra giá trị item
                if (itemPrice <= 0)
                {
                    Console.WriteLine($"WARNING: Item {orderItem.Description} has price {itemPrice}, setting to 1000");
                    itemPrice = 1000; // Default fallback price
                }
                
                if (itemTotal <= 0)
                {
                    Console.WriteLine($"WARNING: Item {orderItem.Description} has total {itemTotal}, recalculating");
                    itemTotal = itemPrice * orderItem.Quantity;
                }
                
                var receiptItem = new ReceiptItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ReceiptId = receipt.Id,
                    DrinkName = orderItem.Description ?? "Unknown",
                    Size = orderItem.Size.ToString(),
                    Quantity = orderItem.Quantity,
                    SugarLevel = orderItem.SugarLevel ?? "100%",
                    IceLevel = orderItem.IceLevel ?? "100%",
                    Toppings = JsonSerializer.Serialize(orderItem.Toppings ?? new List<string>()),
                    UnitPrice = itemPrice,
                    LineTotal = itemTotal
                };
                
                receipt.Items.Add(receiptItem);
                Console.WriteLine($"Adding receipt item: {receiptItem.DrinkName} - {receiptItem.Quantity} x {receiptItem.UnitPrice:F2}đ = {receiptItem.LineTotal:F2}đ");
            }

            // Lưu receipt với transaction để đảm bảo toàn vẹn dữ liệu
            using var transaction = db.Database.BeginTransaction();
            
            try
            {
                // DOUBLE CHECK: Verify receipt data before save
                if (receipt.Total <= 0 || !receipt.Items.Any())
                {
                    Console.WriteLine("ERROR: Receipt validation failed before save");
                    transaction.Rollback();
                    return;
                }
                
                db.Receipts.Add(receipt);
                var changes = db.SaveChanges();
                
                Console.WriteLine($"Saved receipt successfully. Changes: {changes}");
                Console.WriteLine($"Receipt ID: {receipt.Id}, Total: {receipt.Total:F2}, Items: {receipt.Items.Count}");
                
                // VERIFICATION: Kiểm tra dữ liệu sau khi lưu
                var savedReceipt = db.Receipts.FirstOrDefault(r => r.Id == receiptId);
                if (savedReceipt != null)
                {
                    Console.WriteLine($"Verification - Saved receipt: ID={savedReceipt.Id[..8]}, Total={savedReceipt.Total:F2}");
                    
                    if (savedReceipt.Total <= 0)
                    {
                        Console.WriteLine("ERROR: Saved receipt has 0 total! Rolling back...");
                        transaction.Rollback();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("ERROR: Could not verify saved receipt!");
                    transaction.Rollback();
                    return;
                }
                
                // FINAL CHECK: Verify no existing receipts were corrupted
                var zeroTotalReceipts = db.Receipts.Where(r => r.Total <= 0).ToList();
                if (zeroTotalReceipts.Any())
                {
                    Console.WriteLine($"ERROR: Found {zeroTotalReceipts.Count} receipts with 0 total after save!");
                    foreach (var corrupted in zeroTotalReceipts)
                    {
                        Console.WriteLine($"  Corrupted receipt: {corrupted.Id[..8]} - {corrupted.PurchaseTime:dd/MM/yyyy HH:mm}");
                    }
                    
                    // Attempt to fix corrupted receipts
                    foreach (var corrupted in zeroTotalReceipts)
                    {
                        if (corrupted.Items != null && corrupted.Items.Any())
                        {
                            var recalculatedTotal = corrupted.Items.Sum(i => i.LineTotal);
                            if (recalculatedTotal > 0)
                            {
                                Console.WriteLine($"Fixing corrupted receipt {corrupted.Id[..8]}: {corrupted.Total} -> {recalculatedTotal}");
                                corrupted.Total = recalculatedTotal;
                                corrupted.Subtotal = recalculatedTotal;
                            }
                        }
                    }
                    db.SaveChanges();
                }
                
                transaction.Commit();
                
                // Kiểm tra tổng số receipts sau khi thêm
                var newReceiptCount = db.Receipts.Count();
                Console.WriteLine($"Total receipts after save: {newReceiptCount} (increased by {newReceiptCount - existingReceiptCount})");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error during transaction, rolled back: {ex.Message}");
                throw;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving receipt: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Log chi tiết order để debug
            Console.WriteLine($"Order details - ID: {order.Id}, Subtotal: {order.Subtotal}, Total: {order.Total}, Items: {order.Items.Count}");
            foreach (var item in order.Items)
            {
                Console.WriteLine($"  Item: {item.Description}, Price: {item.GetPrice()}, Quantity: {item.Quantity}, Total: {item.LineTotal}");
            }
        }
    }

    public List<Receipt> GetAllReceipts()
    {
        try
        {
            using var db = new MilkTeaDbContext();
            var receipts = db.Receipts
                             .Include(r => r.Items)
                             .OrderByDescending(r => r.PurchaseTime)
                             .AsNoTracking()
                             .ToList();
                             
            Console.WriteLine($"Retrieved {receipts.Count} receipts from database");
            return receipts;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting all receipts: {ex.Message}");
            return new List<Receipt>();
        }
    }

    public List<Receipt> GetReceiptsByDateRange(DateTime from, DateTime to)
    {
        try
        {
            using var db = new MilkTeaDbContext();
            var receipts = db.Receipts
                             .Include(r => r.Items)
                             .Where(r => r.PurchaseTime >= from && r.PurchaseTime <= to)
                             .OrderByDescending(r => r.PurchaseTime)
                             .AsNoTracking()
                             .ToList();
                             
            Console.WriteLine($"Retrieved {receipts.Count} receipts from {from:dd/MM/yyyy} to {to:dd/MM/yyyy}");
            return receipts;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting receipts by date range: {ex.Message}");
            return new List<Receipt>();
        }
    }

    public Receipt? GetReceiptById(string id)
    {
        try
        {
            using var db = new MilkTeaDbContext();
            var receipt = db.Receipts
                            .Include(r => r.Items)
                            .AsNoTracking()
                            .FirstOrDefault(r => r.Id == id);
                            
            Console.WriteLine($"Retrieved receipt by ID {id}: {receipt?.Id ?? "Not found"}");
            return receipt;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting receipt by id: {ex.Message}");
            return null;
        }
    }

    public decimal GetTotalSalesForDate(DateTime date)
    {
        try
        {
            using var db = new MilkTeaDbContext();
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);
            
            var total = db.Receipts
                          .Where(r => r.PurchaseTime >= startOfDay && r.PurchaseTime < endOfDay)
                          .Sum(r => r.Total);
                          
            Console.WriteLine($"Total sales for {date:dd/MM/yyyy}: {total:N0}đ");
            return total;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting total sales for date: {ex.Message}");
            return 0;
        }
    }

    public List<(string ItemName, int Quantity, decimal Revenue)> GetTopSellingItems(int count = 10)
    {
        try
        {
            using var db = new MilkTeaDbContext();
            
            var results = db.ReceiptItems
                            .GroupBy(ri => ri.DrinkName)
                            .Select(g => new
                            {
                                ItemName = g.Key,
                                Quantity = g.Sum(x => x.Quantity),
                                Revenue = g.Sum(x => x.LineTotal)
                            })
                            .OrderByDescending(x => x.Quantity)
                            .Take(count)
                            .AsNoTracking()
                            .ToList()
                            .Select(x => (x.ItemName, x.Quantity, x.Revenue))
                            .ToList();
                            
            Console.WriteLine($"Retrieved top {results.Count} selling items");
            return results;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting top selling items: {ex.Message}");
            return new List<(string, int, decimal)>();
        }
    }
}