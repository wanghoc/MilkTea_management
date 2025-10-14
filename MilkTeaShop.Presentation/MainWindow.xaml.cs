using System.Windows;
using MilkTeaShop.Presentation.ViewModels;

namespace MilkTeaShop.Presentation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainPOSViewModel();
        }
    }
}
