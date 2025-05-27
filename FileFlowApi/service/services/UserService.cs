using FileFlowApi.Data;
using FileFlowApi.Models;
using FileFlowApi.IREPOSITORY;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileFlowApi.Models.DTOs;
using BCrypt.Net;

namespace FileFlowApi.SERVICES
{
    public class UserService : IUserService
    {
        private readonly FileFlowDbContext _context;


        public UserService(FileFlowDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddUserAsync(RegisterDto registerDto)
        {
            // בודק אם כבר יש משתמש עם אותו אימייל
            var existingUser = await GetUserByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                // החזר שגיאה אם יש כבר משתמש עם אותו אימייל
                throw new Exception("Email already in use");
            }

            // בונה את המשתמש החדש עם כל המידע מ-RegisterDto
            var user = new User
            {
                Username = registerDto.UserName,
                Email = registerDto.Email,  // מוודא שהאימייל מועבר כראוי
                PasswordHash = HashPassword(registerDto.Password)  // השימוש ב-HashPassword
            };

            // מוסיף את המשתמש לבסיס הנתונים
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public async Task<List<Category>> GetUserCategoriesAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Categories)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Categories ?? new List<Category>();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync(); // דוגמה לשליפת כל המשתמשים
        }

    }
}
