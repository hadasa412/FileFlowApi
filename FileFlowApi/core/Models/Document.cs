using System;

namespace FileFlowApi.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; } // ✅ חדש

        public string ContentType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public string FilePath { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public bool IsDeleted { get; set; }
    }
}
