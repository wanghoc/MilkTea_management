using System.Windows;
using MilkTeaShop.Infrastructure.Services;
using System.Collections.ObjectModel;
using MilkTeaShop.Domain.Entities;

namespace MilkTeaShop.Presentation;

public partial class SalesReportWindow : Window
{
    public ObservableCollection<DailySalesItem> DailySales { get; } = new();
    public ObservableCollection<ReceiptDisplayItem> ReceiptDetails { get; } = new();
    
    private readonly IEfReceiptService _receiptService;
    private DailySalesItem? _selectedDayItem;

    public SalesReportWindow()
    {
        InitializeComponent();
        DataContext = this;
        _receiptService = new EfReceiptService();
        LoadReportData();
    }

    public DailySalesItem? SelectedDayItem
    {
        get => _selectedDayItem;
        set
        {
            _selectedDayItem = value;
            LoadReceiptDetails();
        }
    }

    private void LoadReportData()
    {
        try
        {
            // Load daily sales for last 30 days
            var receipts = _receiptService.GetReceiptsByDateRange(
                DateTime.Now.AddDays(-30), 
                DateTime.Now);

            var dailySales = receipts
                .GroupBy(r => r.PurchaseTime.Date)
                .Select(g => new DailySalesItem
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(r => r.Total),
                    Receipts = g.OrderByDescending(r => r.PurchaseTime).ToList()
                })
                .OrderByDescending(x => x.Date)
                .ToList();

            DailySales.Clear();
            foreach (var item in dailySales)
            {
                DailySales.Add(item);
            }

            // Update summary
            var totalRevenue = receipts.Sum(r => r.Total);
            var totalOrders = receipts.Count;
            var avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            TotalRevenueText.Text = $"{totalRevenue:N0}đ";
            TotalOrdersText.Text = totalOrders.ToString();
            AvgOrderValueText.Text = $"{avgOrderValue:N0}đ";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải báo cáo: {ex.Message}", "Lỗi", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadReceiptDetails()
    {
        ReceiptDetails.Clear();
        
        if (_selectedDayItem?.Receipts != null)
        {
            foreach (var receipt in _selectedDayItem.Receipts)
            {
                var receiptDisplay = new ReceiptDisplayItem
                {
                    Receipt = receipt,
                    ReceiptId = receipt.Id[..8],
                    PurchaseTime = receipt.PurchaseTimeString,
                    Total = receipt.TotalString,
                    ItemCount = receipt.Items.Count,
                    ItemsSummary = string.Join(", ", receipt.Items.Take(3).Select(i => i.DrinkName)) + 
                                  (receipt.Items.Count > 3 ? "..." : "")
                };
                
                ReceiptDetails.Add(receiptDisplay);
            }
        }
        
        // Update detail header
        if (_selectedDayItem != null)
        {
            DetailHeaderText.Text = $"Hóa đơn ngày {_selectedDayItem.DateString} ({_selectedDayItem.OrderCount} đơn - {_selectedDayItem.RevenueString})";
        }
        else
        {
            DetailHeaderText.Text = "Chọn ngày để xem chi tiết hóa đơn";
        }
    }

    private void DailySalesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (sender is System.Windows.Controls.ListView listView && listView.SelectedItem is DailySalesItem selectedItem)
        {
            SelectedDayItem = selectedItem;
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        LoadReportData();
        ReceiptDetails.Clear();
        DetailHeaderText.Text = "Chọn ngày để xem chi tiết hóa đơn";
    }
}

public class DailySalesItem
{
    public DateTime Date { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
    public List<Receipt> Receipts { get; set; } = new();
    
    public string DateString => Date.ToString("dd/MM/yyyy");
    public string RevenueString => $"{Revenue:N0}đ";
    public string Summary => $"{OrderCount} đơn - {RevenueString}";
}

public class ReceiptDisplayItem
{
    public Receipt? Receipt { get; set; }
    public string ReceiptId { get; set; } = "";
    public string PurchaseTime { get; set; } = "";
    public string Total { get; set; } = "";
    public int ItemCount { get; set; }
    public string ItemsSummary { get; set; } = "";
}