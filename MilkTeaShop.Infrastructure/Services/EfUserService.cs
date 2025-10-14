using MilkTeaShop.Application.Services;
using MilkTeaShop.Domain.Entities;
using MilkTeaShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilkTeaShop.Infrastructure.Services
{
    public class EfUserService : IUserService
    {
        private static bool _databaseInitialized = false;
        private static readonly object _lock = new object();

        public EfUserService()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            lock (_lock)
            {
                if (_databaseInitialized)
                    return;

                try
                {
                    using var db = new MilkTeaDbContext();
                    
                    // Kiểm tra xem database đã tồn tại chưa
                    var dbPath = MilkTeaDbContext.GetDatabasePath();
                    var dbExists = File.Exists(dbPath);
                    
                    Console.WriteLine($"User Database path: {dbPath}");
                    Console.WriteLine($"User Database exists: {dbExists}");
                    
                    // Tạo database nếu chưa tồn tại
                    db.Database.EnsureCreated();
                    
                    // Seed initial users nếu chưa có
                    if (!db.Users.Any())
                    {
                        Console.WriteLine("Seeding initial user data...");
                        SeedInitialUsers(db);
                    }
                    else
                    {
                        Console.WriteLine("Users already exist, skipping seed.");
                    }
                    
                    var userCount = db.Users.Count();
                    Console.WriteLine($"Current user count: {userCount}");
                    
                    _databaseInitialized = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing user database: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }

        private void SeedInitialUsers(MilkTeaDbContext db)
        {
            try
            {
                var users = new List<User>
                {
                    new User 
                    { 
                        Username = "admin", 
                        Password = "admin123", 
                        FullName = "Quản trị viên hệ thống", 
                        Role = UserRole.Admin, 
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new User 
                    { 
                        Username = "manager", 
                        Password = "manager123", 
                        FullName = "Quản lý cửa hàng", 
                        Role = UserRole.Manager, 
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new User 
                    { 
                        Username = "employee1", 
                        Password = "emp123", 
                        FullName = "Nhân viên bán hàng 1", 
                        Role = UserRole.Employee, 
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new User 
                    { 
                        Username = "employee2", 
                        Password = "emp123", 
                        FullName = "Nhân viên bán hàng 2", 
                        Role = UserRole.Employee, 
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    }
                };

                db.Users.AddRange(users);
                db.SaveChanges();
                
                Console.WriteLine($"Seeded {users.Count} users to database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding initial users: {ex.Message}");
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                using var db = new MilkTeaDbContext();
                var users = await db.Users
                                   .AsNoTracking()
                                   .ToListAsync();
                
                Console.WriteLine($"Retrieved {users.Count} users from database");
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all users: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                using var db = new MilkTeaDbContext();
                var user = await db.Users
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(u => u.Id == id);
                
                Console.WriteLine($"Retrieved user by ID {id}: {user?.Username ?? "Not found"}");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user by id: {ex.Message}");
                return null;
            }
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            try
            {
                using var db = new MilkTeaDbContext();
                var user = await db.Users
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(u => 
                                      u.Username.ToLower() == username.ToLower() && 
                                      u.Password == password && 
                                      u.IsActive);
                
                if (user != null)
                {
                    // Update last login date
                    user.LastLoginDate = DateTime.Now;
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                
                Console.WriteLine($"Authentication result for {username}: {(user != null ? "SUCCESS" : "FAILED")}");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error authenticating user: {ex.Message}");
                return null;
            }
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                using var db = new MilkTeaDbContext();
                
                // Ensure proper initialization
                if (user.Id == 0) // Auto-increment will handle this
                {
                    // Let the database handle ID generation
                }
                
                if (user.CreatedDate == DateTime.MinValue)
                {
                    user.CreatedDate = DateTime.Now;
                }
                
                // Validate required fields
                if (string.IsNullOrWhiteSpace(user.Username))
                {
                    throw new ArgumentException("Username cannot be empty");
                }
                
                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    throw new ArgumentException("Password cannot be empty");
                }
                
                if (string.IsNullOrWhiteSpace(user.FullName))
                {
                    throw new ArgumentException("Full name cannot be empty");
                }
                
                // Check if username already exists
                var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == user.Username.ToLower());
                if (existingUser != null)
                {
                    throw new ArgumentException($"Username '{user.Username}' already exists");
                }
                
                db.Users.Add(user);
                await db.SaveChangesAsync();
                
                Console.WriteLine($"Created new user: {user.Username} with ID: {user.Id}");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw new Exception($"Failed to create user: {ex.Message}", ex);
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                using var db = new MilkTeaDbContext();
                db.Users.Update(user);
                await db.SaveChangesAsync();
                
                Console.WriteLine($"Updated user: {user.Username}");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                using var db = new MilkTeaDbContext();
                
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    // Soft delete - chỉ set IsActive = false
                    user.IsActive = false;
                    await db.SaveChangesAsync();
                    
                    Console.WriteLine($"Deactivated user: {user.Username}");
                    return true;
                }
                
                Console.WriteLine($"User not found for deletion: ID {id}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                using var db = new MilkTeaDbContext();
                
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null && user.Password == currentPassword)
                {
                    user.Password = newPassword;
                    await db.SaveChangesAsync();
                    
                    Console.WriteLine($"Changed password for user: {user.Username}");
                    return true;
                }
                
                Console.WriteLine($"Failed to change password for user ID: {userId}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            try
            {
                using var db = new MilkTeaDbContext();
                
                var exists = await db.Users
                                    .AsNoTracking()
                                    .AnyAsync(u => u.Username.ToLower() == username.ToLower() && u.IsActive);
                
                return !exists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking username availability: {ex.Message}");
                return false;
            }
        }
    }
}