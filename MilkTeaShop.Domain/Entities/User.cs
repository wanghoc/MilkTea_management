using System;

namespace MilkTeaShop.Domain.Entities
{
    public enum UserRole
    {
        Admin,
        Manager,
        Employee
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // In real app, this should be hashed
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; }

        public string GetRoleDisplayName()
        {
            return Role switch
            {
                UserRole.Admin => "Quản trị viên",
                UserRole.Manager => "Quản lý",
                UserRole.Employee => "Nhân viên",
                _ => "Không xác định"
            };
        }

        public bool CanManageUsers()
        {
            return Role == UserRole.Admin;
        }

        public bool CanViewReports()
        {
            return Role == UserRole.Admin || Role == UserRole.Manager;
        }

        public bool CanManageMenu()
        {
            return Role == UserRole.Admin || Role == UserRole.Manager;
        }
    }
}