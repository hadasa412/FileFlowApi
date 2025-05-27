using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.DTOs;

namespace core.IRepositories
{
    public interface IReportRepository
    {
        Task<ReportDto> GetSummaryReportAsync();
        Task<List<UploadPerDayDto>> GetUploadsPerDayAsync();

    }

}
