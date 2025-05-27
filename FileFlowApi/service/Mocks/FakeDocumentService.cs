using FileFlowApi.Models;

namespace FileFlowApi.Services
{
    public class FakeDocumentService
    {
        private readonly List<Document> _documents = new List<Document>
        {
            new Document { Id = 1, Title = "מסמך דוגמה", ContentType = "application/pdf", UploadedAt = DateTime.Now, FilePath = "/fake/sample.pdf" },
            new Document { Id = 2, Title = "תמונה לדוגמה", ContentType = "image/png", UploadedAt = DateTime.Now, FilePath = "/fake/image.png" }
        };

        public Task<List<Document>> GetDocumentsAsync()
        {
            return Task.FromResult(_documents);
        }

        public Task<Document> GetDocumentByIdAsync(int id)
        {
            var document = _documents.FirstOrDefault(d => d.Id == id);
            return Task.FromResult(document);
        }
    }
}
