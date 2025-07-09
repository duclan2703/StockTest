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
    public record StockAlertQuery : IQuery<StockAlertQuery, Response<List<StockAlertResponse>>>
    {
        public DateTimeOffset RequestDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
