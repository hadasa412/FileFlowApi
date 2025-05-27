using FileFlowApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FILEFLOW.Core.IRepositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesByUserIdAsync(int userId);
        Task<List<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
    }
}
