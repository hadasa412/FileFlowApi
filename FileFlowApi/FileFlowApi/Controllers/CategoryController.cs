using FILEFLOW.Core.IServices;
using FileFlowApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using core.DTOs;

namespace FileFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // דורש התחברות עם טוקן
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid User ID.");

            var categories = await _categoryService.GetCategoriesByUserIdAsync(userId);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult> AddCategory([FromBody] CategoryDto categoryDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid User ID.");

            var category = new Category
            {
                Name = categoryDto.Name,
                UserId = userId  // קישור למשתמש הנכון
            };

            await _categoryService.AddCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid User ID.");

            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null || category.UserId != userId)
                return NotFound("Category not found or unauthorized.");

            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }


    }
}
