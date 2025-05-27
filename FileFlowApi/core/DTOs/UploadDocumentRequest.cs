using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace core.DTOs
{
    public class UploadDocumentRequest
    {
        public IFormFile File { get; set; }
        public string? CategoryId { get; set; }
        public bool UseAutoTagging { get; set; } // ← הוסף את השורה הזו

    }
}
