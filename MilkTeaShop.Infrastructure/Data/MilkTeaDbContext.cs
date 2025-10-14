using Microsoft.EntityFrameworkCore;
using MilkTeaShop.Domain.Entities;
using System.IO;
using System.Text.Json;

namespace MilkTeaShop.Infrastructure.Data;

public class MilkTeaDbContext : DbContext
{
    // Đặt database trong thư mục dự án
    private static readonly string DbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
    private static readonly string DbPath = Path.Combine(DbFolder, "milktea.db");

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<ReceiptItem> ReceiptItems => Set<ReceiptItem>();
    public DbSet<User> Users => Set<User>(); // Add User table

    public MilkTeaDbContext() { }

    public MilkTeaDbContext(DbContextOptions<MilkTeaDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Tạo thư mục nếu chưa tồn tại
            Directory.CreateDirectory(DbFolder);
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
            
            // Thêm logging để debug
            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User Configuration
        var user = modelBuilder.Entity<User>();
        user.HasKey(x => x.Id);
        user.Property(x => x.Id).ValueGeneratedOnAdd(); // Ensure ID is generated
        user.Property(x => x.Username).HasMaxLength(50).IsRequired();
        user.HasIndex(x => x.Username).IsUnique(); // Prevent duplicate usernames
        user.Property(x => x.Password).HasMaxLength(256).IsRequired();
        user.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        user.Property(x => x.Role).HasConversion<int>();
        user.Property(x => x.IsActive).HasDefaultValue(true);
        user.Property(x => x.CreatedDate).IsRequired();

        // MenuItem Configuration
        var menuItem = modelBuilder.Entity<MenuItem>();
        menuItem.HasKey(x => x.Id);
        menuItem.Property(x => x.Name).HasMaxLength(256).IsRequired();
        menuItem.Property(x => x.Description).HasMaxLength(1024);
        menuItem.Property(x => x.ImagePath).HasMaxLength(512);
        // FIX: Sử dụng TEXT thay vì DECIMAL để tránh precision loss trong SQLite
        menuItem.Property(x => x.BasePrice).HasColumnType("TEXT").HasConversion(
            v => v.ToString("F2"),
            v => decimal.Parse(v)
        );
        menuItem.Property(x => x.Category).HasConversion<int>();
        menuItem.Property(x => x.IsAvailable).HasDefaultValue(true);

        // Receipt Configuration - FIX DATA CORRUPTION
        var receipt = modelBuilder.Entity<Receipt>();
        receipt.HasKey(x => x.Id);
        receipt.Property(x => x.OrderId).HasMaxLength(36).IsRequired();
        receipt.Property(x => x.CreatedAt).IsRequired();
        receipt.Property(x => x.PurchaseTime).IsRequired();
        
        // FIX: Sử dụng TEXT cho decimal để tránh precision loss
        receipt.Property(x => x.Subtotal).HasColumnType("TEXT").HasConversion(
            v => v.ToString("F2"),
            v => string.IsNullOrEmpty(v) ? 0m : decimal.Parse(v)
        );
        receipt.Property(x => x.Discount).HasColumnType("TEXT").HasConversion(
            v => v.ToString("F2"),
            v => string.IsNullOrEmpty(v) ? 0m : decimal.Parse(v)
        );
        receipt.Property(x => x.Total).HasColumnType("TEXT").HasConversion(
            v => v.ToString("F2"),
            v => string.IsNullOrEmpty(v) ? 0m : decimal.Parse(v)
        );
        
        receipt.Property(x => x.CustomerNote).HasMaxLength(1024);
        receipt.Property(x => x.ReceiptContent).HasColumnType("TEXT");
        receipt.Property(x => x.PaymentMethod).HasMaxLength(50);
        
        // Ignore computed properties
        receipt.Ignore(x => x.PurchaseTimeString);
        receipt.Ignore(x => x.DateString);
        receipt.Ignore(x => x.TimeString);
        receipt.Ignore(x => x.TotalString);

        // ReceiptItem Configuration - FIX DATA CORRUPTION
        var receiptItem = modelBuilder.Entity<ReceiptItem>();
        receiptItem.HasKey(x => x.Id);
        receiptItem.Property(x => x.ReceiptId).HasMaxLength(36).IsRequired();
        receiptItem.Property(x => x.DrinkName).HasMaxLength(256).IsRequired();
        receiptItem.Property(x => x.Size).HasMaxLength(20);
        receiptItem.Property(x => x.SugarLevel).HasMaxLength(10);
        receiptItem.Property(x => x.IceLevel).HasMaxLength(10);
        receiptItem.Property(x => x.Toppings).HasMaxLength(1024);
        
        // FIX: Sử dụng TEXT cho decimal để tránh precision loss
        receiptItem.Property(x => x.UnitPrice).HasColumnType("TEXT").HasConversion(
            v => v.ToString("F2"),
            v => string.IsNullOrEmpty(v) ? 0m : decimal.Parse(v)
        );
        receiptItem.Property(x => x.LineTotal).HasColumnType("TEXT").HasConversion(
            v => v.ToString("F2"),
            v => string.IsNullOrEmpty(v) ? 0m : decimal.Parse(v)
        );

        // Ignore computed properties
        receiptItem.Ignore(x => x.UnitPriceString);
        receiptItem.Ignore(x => x.LineTotalString);
        receiptItem.Ignore(x => x.ItemDetails);

        // Configure relationships
        receipt.HasMany(r => r.Items)
               .WithOne(ri => ri.Receipt)
               .HasForeignKey(ri => ri.ReceiptId)
               .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Đảm bảo decimal được handle đúng cách
        configurationBuilder.Properties<decimal>().HaveColumnType("TEXT");
        base.ConfigureConventions(configurationBuilder);
    }

    public static string GetDatabasePath() => DbPath;
    public static string GetDatabaseFolder() => DbFolder;
}