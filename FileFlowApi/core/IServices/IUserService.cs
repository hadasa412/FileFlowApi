using FileFlowApi.Models;
using FileFlowApi.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileFlowApi.IREPOSITORY
{
    public interface IUserService
    {
        Task<User> GetUserByUserNameAsync(string userName);
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(RegisterDto registerDto);
        Task<List<Category>> GetUserCategoriesAsync(int userId);
        Task DeleteUserAsync(int id);
        Task<List<User>> GetAllUsersAsync();

        Task<User> GetUserByEmailAsync(string email); // הוספת הפונקציה החסרה

    }
}
