using System.Threading.Tasks;
using core.DTOs;
using FileFlowApi.Models;  // ודא ש-Class 'Document' נמצא כאן
using Microsoft.AspNetCore.Http;  // נדרש עבור IFormFile

namespace FILEFLOW.Core.IServices  // שנה את ה־namespace ל־Services
{
    public interface IDocumentService  // השתמש ב־public ולא ב־internal
    {
        Task<(byte[] fileContent, string contentType, string fileName)?> DownloadDocumentAsync(int id);
        Task<Document> GetDocumentByIdAsync(int id);
        Task<IEnumerable<DocumentDto>> GetDocumentsByUserIdAsync(int userId);
        Task<List<Document>> GetDocumentsByUserIdAndCategoryIdAsync(int userId, int categoryId);

        Task<Document> UploadDocumentAsync(IFormFile file, int? categoryId, int userId, bool useAutoTagging);
        Task UpdateDocumentAsync(int documentId, UpdateDocumentRequest updateRequest);
        Task DeleteDocumentAsync(int documentId);

    }
}
