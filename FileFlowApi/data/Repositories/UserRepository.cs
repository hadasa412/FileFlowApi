using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.IRepositories;
using FileFlowApi.Data;
using FileFlowApi.Models;
using Microsoft.EntityFrameworkCore;

namespace data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FileFlowDbContext _context;

        public UserRepository(FileFlowDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUserNameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        // הוספת הפונקציה החדשה לחיפוש לפי אימייל
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
