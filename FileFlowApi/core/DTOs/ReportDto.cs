using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.DTOs
{
    public class ReportDto
    {
        public int TotalDocuments { get; set; }
        public int TotalUsers { get; set; }
        public int MostActiveUserId { get; set; }
        public string MostUsedCategory { get; set; }
    }

}
