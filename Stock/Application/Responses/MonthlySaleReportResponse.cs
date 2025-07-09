using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Responses
{
    public class MonthlySaleReportResponse
    {
        public string? ReportMonth { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalAmount { get; set; }
        public string? TopSaleProduct { get; set; }
        public decimal GrowthRate { get; set; }
    }
}
