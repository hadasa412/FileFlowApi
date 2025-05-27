using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using core.Models;
using System;
using FileFlowApi.Data;

namespace FileFlowApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly FileFlowDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly string myApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        public AiController(FileFlowDbContext dbContext, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
        }


        [HttpPost("categorize")]
        public async Task<IActionResult> CategorizeDocument([FromForm] CategorizationRequest request)
        {
            try
            {
                // קריאת תוכן הקובץ
                string fileContent;
                using (var reader = new StreamReader(request.File.OpenReadStream()))
                {
                    fileContent = await reader.ReadToEndAsync();
                }

                // שליפת הקטגוריות של המשתמש
                var userCategories = await _dbContext.Categories
                    .Where(c => c.UserId == request.UserId)
                    .ToListAsync();

                if (!userCategories.Any())
                    return BadRequest("לא נמצאו קטגוריות למשתמש.");

                // בניית פרומפט ל-AI
                var prompt = new
                {
                    model = "gpt-4o-mini",
                    messages = new[]
                    {
                new { role = "system", content = "אתה מערכת לסיווג מסמכים לפי קטגוריות שניתנו." },
                new
                {
                    role = "user",
                    content = $"המסמך הבא:\n{fileContent}\n\nהקטגוריות האפשריות הן:\n{string.Join(", ", userCategories.Select(c => c.Name))}\n\nאיזו קטגוריה הכי מתאימה למסמך הזה? ענה בשם הקטגוריה בלבד."
                }
            }
                };

                // קריאה ל-OpenAI
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
                {
                    Content = JsonContent.Create(prompt)
                };
                requestMessage.Headers.Add("Authorization", $"Bearer {myApiKey}");

                var response = await _httpClient.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode(500, $"שגיאה מה-API: {errorContent}");
                }

                // חילוץ שם הקטגוריה מתוך תגובת ה-JSON
                var resultJson = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
                var categoryName = resultJson?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

                if (string.IsNullOrWhiteSpace(categoryName))
                    return BadRequest("לא התקבלה קטגוריה מתאימה מה-AI.");

                // חיפוש הקטגוריה במסד
                var category = userCategories.FirstOrDefault(c =>
                    string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase));

                if (category == null)
                    return NotFound("הקטגוריה שהוחזרה אינה קיימת בקטגוריות של המשתמש.");

                return Ok(new
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"שגיאה כללית: {ex.Message}");
            }
        }

    }
}
