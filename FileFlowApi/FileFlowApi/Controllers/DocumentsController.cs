using FileFlowApi.Models;
using FileFlowApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using core.DTOs;
using core.IServices;
using FILEFLOW.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FileFlowApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IS3Service _s3Service;

        public DocumentsController(IDocumentService documentService, IS3Service s3Service)
        {
            _documentService = documentService;
            _s3Service = s3Service;
        }

        [Authorize]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            int? categoryId = null;

            if (!request.UseAutoTagging)
            {
                if (string.IsNullOrEmpty(request.CategoryId) || !int.TryParse(request.CategoryId, out int parsedCategoryId))
                {
                    return BadRequest("יש לספק מזהה קטגוריה תקין אם לא נבחר מיון אוטומטי.");
                }

                categoryId = parsedCategoryId;
            }

            try
            {
                await _documentService.UploadDocumentAsync(request.File, categoryId, userId, request.UseAutoTagging);
                return Ok(new { message = "Document uploaded successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("download-url/{fileName}")]
        public async Task<IActionResult> GetDownloadUrl(string fileName)
        {
            var url = await _s3Service.GetDownloadUrlAsync(fileName);
            return Ok(new { downloadUrl = url });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
                return NotFound("המסמך לא נמצא");

            return Ok(document);
        }

        [Authorize]
        [HttpGet("user-documents")]
        public async Task<IActionResult> GetUserDocuments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var documents = await _documentService.GetDocumentsByUserIdAsync(userId);
            return Ok(documents);
        }

        [Authorize]
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetDocumentsByCategory(int categoryId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var documents = await _documentService.GetDocumentsByUserIdAndCategoryIdAsync(userId, categoryId);
            return Ok(documents);
        }

        // חדש: מחיקת מסמך
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);
                if (document == null || document.UserId != userId)
                    return NotFound("Document not found or unauthorized.");

                // מחיקת הקובץ מה-S3
                await _s3Service.DeleteFileAsync(document.FilePath);

                // מחיקת המסמך מה-DB
                await _documentService.DeleteDocumentAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[Authorize]
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateDocument(int id, [FromBody] UpdateDocumentRequest request)
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        //    {
        //        return Unauthorized("User ID not found in token.");
        //    }

        //    var document = await _documentService.GetDocumentByIdAsync(id);
        //    if (document == null)
        //    {
        //        return NotFound("Document not found.");
        //    }

        //    if (document.UserId != userId)
        //    {
        //        return Forbid("You are not authorized to update this document.");
        //    }

        //    try
        //    {
        //        await _documentService.UpdateDocumentAsync(id, request);
        //        return Ok("Document updated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //private readonly IAIService _aiService;

        //[Authorize]
        //[HttpPost("upload")]
        //[Consumes("multipart/form-data")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request, [FromQuery] bool autoTagging = true)
        //{
        //    if (string.IsNullOrEmpty(request.CategoryId) || !int.TryParse(request.CategoryId, out int categoryId))
        //    {
        //        return BadRequest("Invalid or missing category ID.");
        //    }

        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        //    {
        //        return Unauthorized("User ID not found in token.");
        //    }

        //    try
        //    {
        //        // אם יש לבחור בתיוג אוטומטי, נקרא לפונקציה של ה-AI
        //        if (autoTagging)
        //        {
        //            // הפעלת תיוג אוטומטי באמצעות AI
        //            var tags = await _aiService.TagDocumentAsync(request.File);
        //            // אפשר להוסיף את התיוגים למסמך או לבצע פעולה אחרת
        //        }

        //        await _documentService.UploadDocumentAsync(request.File, categoryId, userId);
        //        return Ok("Document uploaded successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}



    }
}
