using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace core.Models
{
    public class CategorizationRequest
    {
        public IFormFile File { get; set; }
        public int UserId { get; set; }
    }


}
