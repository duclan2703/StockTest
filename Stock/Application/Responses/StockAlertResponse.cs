using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Responses
{
    public class StockAlertResponse
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int StockQtty { get; set; }
        public double AvgSoldLast3Months { get; set; }
        public double ExpectedShortage { get; set; }
    }
}
