using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace core.IServices
{

    public interface IS3Service
    {
        Task<string> UploadFileAsync(IFormFile file, string fileName);
        Task<string> GetDownloadUrlAsync(string fileName);
        Task<bool> DeleteFileAsync(string fileName);

    }

}

