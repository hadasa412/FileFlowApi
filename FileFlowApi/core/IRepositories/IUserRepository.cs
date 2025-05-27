using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileFlowApi.Models;



namespace core.IRepositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByUserNameAsync(string username);
        Task AddUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
    }
}
