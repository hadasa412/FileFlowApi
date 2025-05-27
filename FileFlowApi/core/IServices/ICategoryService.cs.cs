using FileFlowApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FILEFLOW.Core.IServices
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesByUserIdAsync(int userId);
        Task<List<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        //Task<string?> CategorizeDocumentAsync(string content, int userId);
        Task<string> CategorizeDocumentAsync(string fileContent, List<string> userCategories);


    }
}
