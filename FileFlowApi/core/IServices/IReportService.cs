using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.DTOs;
using core.Models;

namespace core.IServices
{
    public interface IReportService
    {
        Task<List<Report>> GetAllReportsAsync();
        Task<Report> GetReportByIdAsync(int reportId);
        Task CreateReportAsync(Report report);
        Task UpdateReportAsync(int reportId, Report report);
        Task DeleteReportAsync(int reportId);
        Task<ReportDto> GetSummaryReportAsync();
        Task<List<UploadPerDayDto>> GetUploadsPerDayAsync();

    }


}
