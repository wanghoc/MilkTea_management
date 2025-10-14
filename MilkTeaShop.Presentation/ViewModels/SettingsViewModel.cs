using System.Collections.ObjectModel;
using System.Windows;
using MilkTeaShop.Domain.Entities;
using MilkTeaShop.Domain.ValueObjects;
using MilkTeaShop.Application.Services;
using MilkTeaShop.Infrastructure.Services;

namespace MilkTeaShop.Presentation.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly IMenuService _menuService;
    private int _selectedTabIndex = 0;
    private MenuItem? _selectedItem;

    public ObservableCollection<MenuItem> MilkTeaItems { get; } = new();
    public ObservableCollection<MenuItem> ToppingItems { get; } = new();

    public RelayCommand AddNewItemCommand { get; private set; }
    public RelayCommand EditItemCommand { get; private set; }
    public RelayCommand DeleteItemCommand { get; private set; }

    public SettingsViewModel()
    {
        try
        {
            _menuService = new EfMenuService(); // Sử dụng SQLite database
            
            AddNewItemCommand = new RelayCommand(AddNewItem);
            EditItemCommand = new RelayCommand(EditItem);
            DeleteItemCommand = new RelayCommand(DeleteItem);
            
            LoadMenuItems();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khởi tạo cài đặt: {ex.Message}", "Lỗi nghiêm trọng", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            _selectedTabIndex = value;
            OnPropertyChanged();
        }
    }

    public MenuItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    private void AddNewItem(object? parameter)
    {
        try
        {
            var addItemWindow = new AddEditItemWindow();
            
            // Safer owner assignment
            if (System.Windows.Application.Current?.MainWindow != null)
            {
                addItemWindow.Owner = System.Windows.Application.Current.MainWindow;
            }
            
            addItemWindow.Title = "Thêm món mới";

            var viewModel = addItemWindow.DataContext as AddEditItemViewModel;
            if (viewModel != null)
            {
                var category = SelectedTabIndex == 0 ? MenuCategory.MilkTea : MenuCategory.Topping;
                viewModel.SetCategory(category);
                viewModel.SetMenuService(_menuService); // Pass the database service
            }

            if (addItemWindow.ShowDialog() == true)
            {
                LoadMenuItems(); // Auto refresh after adding
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi thêm món mới: {ex.Message}", "Lỗi", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void EditItem(object? parameter)
    {
        try
        {
            if (parameter is not MenuItem item) 
            {
                MessageBox.Show("Không thể xác định món cần chỉnh sửa!", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editItemWindow = new AddEditItemWindow();
            
            // Safer owner assignment
            if (System.Windows.Application.Current?.MainWindow != null)
            {
                editItemWindow.Owner = System.Windows.Application.Current.MainWindow;
            }
            
            editItemWindow.Title = "Chỉnh sửa món";

            var viewModel = editItemWindow.DataContext as AddEditItemViewModel;
            if (viewModel != null)
            {
                viewModel.SetMenuService(_menuService); // Pass the database service
                viewModel.LoadItem(item);
                Console.WriteLine($"Loading item for edit: {item.Name}, Price: {item.BasePrice}");
            }
            else
            {
                MessageBox.Show("Không thể khởi tạo form chỉnh sửa!", "Lỗi", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = editItemWindow.ShowDialog();
            if (result == true)
            {
                Console.WriteLine("Edit completed successfully");
                LoadMenuItems(); // Auto refresh after editing
                
                // Force UI refresh
                OnPropertyChanged(nameof(MilkTeaItems));
                OnPropertyChanged(nameof(ToppingItems));
            }
            else
            {
                Console.WriteLine("Edit was cancelled");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi chỉnh sửa món: {ex.Message}\n\nStackTrace: {ex.StackTrace}", "Lỗi", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DeleteItem(object? parameter)
    {
        try
        {
            if (parameter is not MenuItem item) return;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa '{item.Name}'?\nThao tác này không thể hoàn tác.",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                CleanupItemImage(item);
                _menuService.RemoveItem(item);
                
                // Remove from UI collections immediately
                var collection = item.Category == MenuCategory.MilkTea ? MilkTeaItems : ToppingItems;
                collection.Remove(item);

                MessageBox.Show($"Đã xóa '{item.Name}' thành công!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception deleteEx)
            {
                MessageBox.Show($"Lỗi khi xóa món: {deleteEx.Message}", "Lỗi xóa",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi thao tác xóa: {ex.Message}", "Lỗi", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadMenuItems()
    {
        try
        {
            MilkTeaItems.Clear();
            ToppingItems.Clear();

            var milkTeaItems = _menuService?.GetMilkTeaItems() ?? new List<MenuItem>();
            var toppingItems = _menuService?.GetToppingItems() ?? new List<MenuItem>();

            Console.WriteLine($"Loading {milkTeaItems.Count} milk tea items and {toppingItems.Count} topping items");

            foreach (var item in milkTeaItems)
            {
                MilkTeaItems.Add(item);
                Console.WriteLine($"Loaded milk tea: {item.Name} - {item.BasePrice}");
            }

            foreach (var item in toppingItems)
            {
                ToppingItems.Add(item);
                Console.WriteLine($"Loaded topping: {item.Name} - {item.BasePrice}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách món trong cài đặt: {ex.Message}", "Lỗi", 
                           MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private static void CleanupItemImage(MenuItem item)
    {
        if (string.IsNullOrEmpty(item.ImagePath) || !item.ImagePath.StartsWith("Images"))
            return;

        try
        {
            var fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, item.ImagePath);
            if (!System.IO.File.Exists(fullPath)) return;

            // For database version, we should check with the database service
            // instead of StaticMenuData, but this is a reasonable fallback
            System.IO.File.Delete(fullPath);
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}