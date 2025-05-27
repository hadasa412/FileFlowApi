using Amazon.S3;
using Amazon.S3.Model;
using FileFlowApi.Data;
using FileFlowApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using FILEFLOW.Core.IServices;
using core.DTOs;
using Microsoft.AspNetCore.Mvc;



namespace FileFlowApi.Services  // וודא שה־namespace זהה לממשק
{
    public class DocumentService : IDocumentService  // וודא שהמחלקה מממשת את הממשק
    {
        private readonly FileFlowDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IAmazonS3 _s3Client;
        private readonly ICategoryService _categoryService; // הוספנו את ה-CategoryService
        public DocumentService(FileFlowDbContext context, IWebHostEnvironment environment, IAmazonS3 s3Client, ICategoryService categoryService)
        {
            _context = context;
            _environment = environment;
            _s3Client = s3Client;
            _categoryService = categoryService; // אתחול המשתנה
        }
        public async Task<Document> UploadDocumentAsync(IFormFile file, int? categoryId, int userId, bool useAutoTagging)
        {
            Console.WriteLine($"UploadDocumentAsync: קיבל categoryId={categoryId}, userId={userId}, useAutoTagging={useAutoTagging}");

            if (file == null || file.Length == 0)
                throw new ArgumentException("קובץ ריק או לא תקין");

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var bucketName = "user-files-fileflow";
            var key = "documents/" + uniqueFileName;

            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };

            var response = await _s3Client.PutObjectAsync(putRequest);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("העלאת הקובץ ל-S3 נכשלה");
                throw new Exception("העלאת הקובץ ל-S3 נכשלה");
            }

            // ברירת מחדל לקטגוריה: הערך שהתקבל, יכול להיות 0 (כלומר לא נבחרה קטגוריה ידנית)
            var finalCategoryId = categoryId;

            var userCategories = await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var categoryNames = userCategories.Select(c => c.Name).ToList();

            if (useAutoTagging)
            {
                Console.WriteLine("תיוג אוטומטי מופעל - קורא ל-AI...");
                using var reader = new StreamReader(file.OpenReadStream());
                var content = await reader.ReadToEndAsync();

                var suggestedCategoryName = await _categoryService.CategorizeDocumentAsync(content, categoryNames);

                if (!string.IsNullOrWhiteSpace(suggestedCategoryName))
                {
                    var userCategory = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name == suggestedCategoryName && c.UserId == userId);

                    if (userCategory != null)
                    {
                        finalCategoryId = userCategory.Id;
                        Console.WriteLine($"הקטגוריה עודכנה אוטומטית ל: {userCategory.Name}");
                    }
                    else
                    {
                        Console.WriteLine("ה-AI הציע קטגוריה אך לא נמצאה תואמת בבסיס הנתונים.");
                    }
                }
                else
                {
                    Console.WriteLine("ה-AI לא הציע קטגוריה.");
                }
            }
            else
            {
                Console.WriteLine("תיוג אוטומטי לא נבחר – נשמר לפי קטגוריה ידנית.");
            }

            // אם לא בחרו תיוג אוטומטי וקטגוריה = 0, זרוק שגיאה
            if (!useAutoTagging && finalCategoryId == 0)
            {
                throw new ArgumentException("CategoryId is required if not using auto tagging.");
            }

            var document = new Document
            {
                Title = file.FileName,
                ContentType = file.ContentType,
                UploadedAt = DateTime.UtcNow,
                FilePath = key,
                CategoryId = finalCategoryId.Value,
                UserId = userId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }


        public async Task<(byte[] fileContent, string contentType, string fileName)?> DownloadDocumentAsync(int id)
        {
            var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
            if (document == null || string.IsNullOrEmpty(document.FilePath))
                return null;

            var getRequest = new GetObjectRequest
            {
                BucketName = "user-files-fileflow",
                Key = document.FilePath
            };

            var response = await _s3Client.GetObjectAsync(getRequest);

            using (var memoryStream = new MemoryStream())
            {
                await response.ResponseStream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return (fileBytes, document.ContentType, document.Title);
            }
        }

        public async Task<Document> GetDocumentByIdAsync(int id)
        {
            return await _context.Documents.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentsByUserIdAsync(int userId)
        {
            return await _context.Documents
                .Where(d => d.UserId == userId)
               .Select(d => new DocumentDto
               {
                   Id = d.Id,
                   Title = d.Title, // שינוי מ- FileName ל- Title
                   CategoryId = d.CategoryId,
                   UploadedAt = d.UploadedAt // שינוי מ- UploadDate ל- UploadedAt
               })
                .ToListAsync();
        }
        public async Task<List<Document>> GetDocumentsByUserIdAndCategoryIdAsync(int userId, int categoryId)
        {
            return await _context.Documents
                .Where(d => d.UserId == userId && d.CategoryId == categoryId)
                .ToListAsync();
        }



        public async Task UpdateDocumentAsync(int documentId, UpdateDocumentRequest request)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null)
                throw new Exception("Document not found.");

            if (!string.IsNullOrEmpty(request.NewFileName))
                document.Title = request.NewFileName;

            if (!string.IsNullOrEmpty(request.NewDescription))
                document.Description = request.NewDescription;

            if (request.NewCategoryId.HasValue)
                document.CategoryId = request.NewCategoryId.Value;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(int documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null)
                throw new Exception("Document not found.");

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }


    }
}
