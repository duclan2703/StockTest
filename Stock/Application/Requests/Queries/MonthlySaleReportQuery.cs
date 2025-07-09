using Application.Responses;
using Core;
using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.Queries
{
    public record MonthlySaleReportQuery : IQuery<MonthlySaleReportQuery, Response<MonthlySaleReportResponse>>
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
