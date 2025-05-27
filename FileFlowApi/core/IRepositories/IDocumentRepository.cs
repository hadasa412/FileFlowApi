using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileFlowApi.Models;

namespace FILEFLOW.Core.IRepositories
{
    public interface IDocumentRepository
    {
        Task<Document> GetDocumentByIdAsync(int id);
        Task AddDocumentAsync(Document document);
        Task SaveChangesAsync();
    }
}

