using System.Text;
using System.Text.Json;
using core.IServices;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using core.Models;
using System.Net.Http.Json;

public class AIService : IAIService
{

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    public AIService(IConfiguration configuration, HttpClient httpClient) // שנה גם את זה
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public async Task<string> CategorizeDocumentAsync(string fileContent, List<string> userCategories)
    {
        var apiKey = _configuration["OPENAI_API_KEY"]; // ✅ תוקן!
        Console.WriteLine($"API KEY = {!string.IsNullOrEmpty(apiKey)}"); // לא להדפיס את המפתח האמיתי!
        
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new UnauthorizedAccessException("OPENAI_API_KEY לא נמצא בהגדרות");
        }
        
        var prompt = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = "אתה מערכת לסיווג מסמכים לפי קטגוריות שניתנו." },
                new {
                    role = "user",
                    content = $"המסמך הבא:\n{fileContent}\n\nהקטגוריות האפשריות הן:\n{string.Join(", ", userCategories)}\n\nאיזו קטגוריה הכי מתאימה למסמך הזה? ענה בשם הקטגוריה בלבד."
                }
            }
        };
        
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = JsonContent.Create(prompt)
        };
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        
        try
        {
            var response = await _httpClient.SendAsync(request);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("מפתח OpenAI לא תקין או פג תוקף");
            }
            
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
            return result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"שגיאה בקריאה ל-OpenAI: {ex.Message}");
            throw;
        }
    }
}
