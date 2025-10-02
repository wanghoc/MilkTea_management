using MilkTeaShop.Infrastructure.Data;
using MilkTeaShop.Infrastructure.Services;
using MilkTeaShop.Application.Services;
using System;
using System.Threading.Tasks;

namespace MilkTeaShop.Infrastructure.Services
{
    public class DatabaseInitializationService
    {
        public static async Task InitializeDatabase()
        {
            try
            {
                Console.WriteLine("Starting database initialization...");

                // Initialize Users table
                var userService = new EfUserService();
                var users = await userService.GetAllUsersAsync();
                Console.WriteLine($"Users in database: {users.Count()}");

                // Initialize Menu items
                var menuService = new EfMenuService();
                var menuItems = await menuService.GetAllItemsAsync();
                Console.WriteLine($"Menu items in database: {menuItems.Count()}");

                // Check database connection
                using var context = new MilkTeaDbContext();
                var canConnect = await context.Database.CanConnectAsync();
                Console.WriteLine($"Database connection status: {canConnect}");

                if (canConnect)
                {
                    var dbPath = MilkTeaDbContext.GetDatabasePath();
                    Console.WriteLine($"Database location: {dbPath}");
                    
                    // Check if database file exists
                    if (File.Exists(dbPath))
                    {
                        var fileSize = new FileInfo(dbPath).Length;
                        Console.WriteLine($"Database file size: {fileSize} bytes");
                    }
                }

                Console.WriteLine("Database initialization completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database initialization: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public static async Task<DatabaseStatus> CheckDatabaseStatus()
        {
            try
            {
                using var context = new MilkTeaDbContext();
                
                var status = new DatabaseStatus();
                status.CanConnect = await context.Database.CanConnectAsync();
                
                if (status.CanConnect)
                {
                    status.UserCount = context.Users.Count();
                    status.MenuItemCount = context.MenuItems.Count();
                    status.ReceiptCount = context.Receipts.Count();
                    status.DatabasePath = MilkTeaDbContext.GetDatabasePath();
                    
                    if (File.Exists(status.DatabasePath))
                    {
                        status.DatabaseSizeBytes = new FileInfo(status.DatabasePath).Length;
                    }
                }

                return status;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking database status: {ex.Message}");
                return new DatabaseStatus { CanConnect = false, ErrorMessage = ex.Message };
            }
        }
    }

    public class DatabaseStatus
    {
        public bool CanConnect { get; set; }
        public int UserCount { get; set; }
        public int MenuItemCount { get; set; }
        public int ReceiptCount { get; set; }
        public string DatabasePath { get; set; } = string.Empty;
        public long DatabaseSizeBytes { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public string DatabaseSizeFormatted => DatabaseSizeBytes > 0 ? $"{DatabaseSizeBytes / 1024.0:F1} KB" : "0 KB";
    }
}