using MilkTeaShop.Domain.Entities;
using System;

namespace MilkTeaShop.Presentation.Auth
{
    public sealed class CurrentUser
    {
        private static CurrentUser? _instance;
        private static readonly object _lock = new object();

        public static CurrentUser Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new CurrentUser();
                    }
                }
                return _instance;
            }
        }

        private CurrentUser() { }

        public User? LoggedInUser { get; private set; }
        public bool IsLoggedIn => LoggedInUser != null;

        public void Login(User user)
        {
            LoggedInUser = user;
            user.LastLoginDate = DateTime.Now;
            Console.WriteLine($"User logged in: {user.FullName} ({user.Role})");
        }

        public void Logout()
        {
            if (LoggedInUser != null)
            {
                Console.WriteLine($"User logged out: {LoggedInUser.FullName}");
                LoggedInUser = null;
            }
        }

        public string GetRoleDisplayName()
        {
            return LoggedInUser?.GetRoleDisplayName() ?? "Chưa đăng nhập";
        }

        public bool HasPermission(UserRole requiredRole)
        {
            if (!IsLoggedIn) return false;
            
            return LoggedInUser!.Role switch
            {
                UserRole.Admin => true, // Admin has all permissions
                UserRole.Manager => requiredRole != UserRole.Admin, // Manager can't access admin features
                UserRole.Employee => requiredRole == UserRole.Employee, // Employee only has employee permissions
                _ => false
            };
        }
    }
}