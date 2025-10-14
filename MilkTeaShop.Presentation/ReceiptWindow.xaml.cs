using System.Windows;

namespace MilkTeaShop.Presentation;

public partial class ReceiptWindow : Window
{
    public ReceiptWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public static readonly DependencyProperty ReceiptTextProperty =
        DependencyProperty.Register(nameof(ReceiptText), typeof(string), typeof(ReceiptWindow), new PropertyMetadata(string.Empty));

    public string ReceiptText
    {
        get => (string)GetValue(ReceiptTextProperty);
        set => SetValue(ReceiptTextProperty, value);
    }

    public void SetReceiptContent(string receiptContent)
    {
        ReceiptText = receiptContent;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Print_Click(object sender, RoutedEventArgs e)
    {
        // Trong thực tế sẽ in hóa đơn
        MessageBox.Show("Chức năng in đang được phát triển!", "Thông báo", 
                       MessageBoxButton.OK, MessageBoxImage.Information);
    }
}