using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileFlowApi.Models.DTOs;
using FileFlowApi.Models;
using Microsoft.AspNetCore.Http;

namespace core.IServices
{
    public interface IAIService
    {
        //Task<string> ClassifyDocumentAsync(string fileContent, List<string> userCategories);
        Task<string> CategorizeDocumentAsync(string fileContent, List<string> userCategories);


    }
}

