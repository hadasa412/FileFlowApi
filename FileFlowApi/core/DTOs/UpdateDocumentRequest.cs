using System.ComponentModel.DataAnnotations;

namespace core.DTOs
{
    public class UpdateDocumentRequest
    {
        [Required(ErrorMessage = "Title is required.")]
        public string? NewFileName { get; set; }

        public string? NewDescription { get; set; }

        public int? NewCategoryId { get; set; }
    }
}
