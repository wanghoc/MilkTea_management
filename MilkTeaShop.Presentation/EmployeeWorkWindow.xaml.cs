using System.Windows;
using MilkTeaShop.Presentation.ViewModels;
using MilkTeaShop.Domain.Entities;
using MilkTeaShop.Presentation.Auth; // Add this line for CurrentUser
using System;
using System.Linq;

namespace MilkTeaShop.Presentation
{
    public partial class EmployeeWorkWindow : Window
    {
        public EmployeeWorkWindow()
        {
            Console.WriteLine("Starting EmployeeWorkWindow initialization...");
            
            try
            {
                // Initialize window first
                InitializeComponent();
                Console.WriteLine("Window components initialized");
                
                // Force maximize window
                this.WindowState = WindowState.Maximized;
                Console.WriteLine("Window state set to Maximized");
                
                // Verify logged in user (but don't crash if not found)
                if (!CurrentUser.Instance.IsLoggedIn || CurrentUser.Instance.LoggedInUser == null)
                {
                    Console.WriteLine("WARNING: No logged-in user when EmployeeWorkWindow constructor called");
                    // Don't restart application, just show a warning
                    MessageBox.Show("Không tìm thấy thông tin đăng nhập. Vui lòng đăng nhập lại.", 
                                   "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Console.WriteLine($"Logged in user confirmed: {CurrentUser.Instance.LoggedInUser.FullName} ({CurrentUser.Instance.LoggedInUser.Role})");
                    
                    // Update UI for current user
                    UpdateUIForCurrentUser();
                    Console.WriteLine("UI updated for current user");
                }
                
                // Set data context for binding
                try
                {
                    DataContext = new MainPOSViewModel();
                    Console.WriteLine("DataContext set to MainPOSViewModel");
                }
                catch (Exception viewEx)
                {
                    Console.WriteLine($"Warning: View model initialization failed: {viewEx.Message}");
                    MessageBox.Show($"Lỗi khởi tạo giao diện POS: {viewEx.Message}\n\nMột số chức năng có thể không hoạt động.",
                                  "Cảnh báo giao diện", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
                Console.WriteLine("EmployeeWorkWindow initialization completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in EmployeeWorkWindow constructor: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Don't restart application, just show error and continue
                MessageBox.Show($"Có lỗi khi khởi tạo cửa sổ nhân viên: {ex.Message}\n\nỨng dụng sẽ tiếp tục hoạt động với chức năng hạn chế.", 
                              "Lỗi khởi tạo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RestartApplication()
        {
            try
            {
                Console.WriteLine("Restarting application...");
                CurrentUser.Instance.Logout(); // Clear user session
                System.Windows.Application.Current.Shutdown();
                System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!);
            }
            catch
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void UpdateUIForCurrentUser()
        {
            if (!CurrentUser.Instance.IsLoggedIn) return;

            var user = CurrentUser.Instance.LoggedInUser!;
            
            // Update welcome text
            var welcomeText = this.FindName("WelcomeText") as System.Windows.Controls.TextBlock;
            var roleText = this.FindName("RoleText") as System.Windows.Controls.TextBlock;
            
            if (welcomeText != null)
                welcomeText.Text = $"Xin chào, {user.FullName}";
            
            if (roleText != null)
                roleText.Text = $"Vai trò: {CurrentUser.Instance.GetRoleDisplayName()}";
            
            Console.WriteLine($"Employee interface updated for: {user.FullName} ({user.Role})");
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var changePasswordWindow = new ChangePasswordWindow();
                changePasswordWindow.Owner = this;
                changePasswordWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở cửa sổ đổi mật khẩu: {ex.Message}", 
                               "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất?", 
                "Xác nhận đăng xuất",
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);
                
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Console.WriteLine("Employee logging out...");
                    CurrentUser.Instance.Logout();
                    
                    // Close current window and restart app with login
                    RestartApplication();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during logout: {ex.Message}");
                    MessageBox.Show($"Lỗi đăng xuất: {ex.Message}", 
                                   "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}