using System.Windows;
using MilkTeaShop.Presentation.ViewModels;

namespace MilkTeaShop.Presentation;

public partial class AddEditItemWindow : Window
{
    public AddEditItemWindow()
    {
        InitializeComponent();
        DataContext = new AddEditItemViewModel(this);
    }
}