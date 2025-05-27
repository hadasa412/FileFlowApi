using FileFlowApi.Models;
using FILEFLOW.Core.IServices;
using FILEFLOW.Core.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using core.IServices;

namespace FileFlowApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAIService _aiService;
        public CategoryService(ICategoryRepository categoryRepository, IAIService aiService)
        {
            _categoryRepository = categoryRepository;
            _aiService = aiService;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _categoryRepository.GetCategoriesAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await _categoryRepository.GetCategoryByIdAsync(categoryId);
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _categoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<List<Category>> GetCategoriesByUserIdAsync(int userId)
        {
            return await _categoryRepository.GetCategoriesByUserIdAsync(userId);
        }

        public async Task<string> CategorizeDocumentAsync(string fileContent, List<string> userCategories)
        {
            return await _aiService.CategorizeDocumentAsync(fileContent, userCategories);
        }

    }
}
    