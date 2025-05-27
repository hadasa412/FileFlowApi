using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.DTOs;
using core.IServices;
using core.Models;
using FileFlowApi.Data;
using Microsoft.EntityFrameworkCore;

namespace service.services
{
    public class ReportService : IReportService
    {
        private readonly FileFlowDbContext _context;

        public ReportService(FileFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<Report>> GetAllReportsAsync()
        {
            return await _context.Reports.ToListAsync();
        }

        public async Task<Report> GetReportByIdAsync(int reportId)
        {
            return await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == reportId);
        }

        public async Task CreateReportAsync(Report report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReportAsync(int reportId, Report report)
        {
            var existingReport = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (existingReport == null)
                throw new KeyNotFoundException("Report not found.");

            existingReport.Title = report.Title;
            existingReport.Content = report.Content;
            existingReport.CreatedAt = report.CreatedAt;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteReportAsync(int reportId)
        {
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                throw new KeyNotFoundException("Report not found.");

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
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
