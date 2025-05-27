using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.DTOs;
using FileFlowApi.Data;
using Microsoft.EntityFrameworkCore;
using core.IRepositories;


namespace data.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly FileFlowDbContext _context;

        public ReportRepository(FileFlowDbContext context)
        {
            _context = context;
        }

        public async Task<ReportDto> GetSummaryReportAsync()
        {
            var totalDocs = await _context.Documents.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var mostUsedCategory = await _context.Documents
                .GroupBy(d => d.CategoryId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key.ToString())
                .FirstOrDefaultAsync();

            var mostActiveUserId = await _context.Documents
                .GroupBy(d => d.UserId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            return new ReportDto
            {
                TotalDocuments = totalDocs,
                TotalUsers = totalUsers,
                MostActiveUserId = mostActiveUserId,
                MostUsedCategory = mostUsedCategory
            };
        }

        public async Task<List<UploadPerDayDto>> GetUploadsPerDayAsync()
        {
            return await _context.Documents
                .GroupBy(d => d.UploadedAt.Date)
                .Select(g => new UploadPerDayDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }
    }

}
