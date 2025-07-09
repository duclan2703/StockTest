using Application.Requests.Queries;
using Application.Responses;
using Core;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Stock.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Application.Handlers
{
    public class MonthlySaleReportQueryHandler : IQueryHandler<MonthlySaleReportQuery, Response<MonthlySaleReportResponse>>
    {
        private readonly AdventureWorksDbContext _dbContext;
        private readonly IMemoryCache _cache;

        public MonthlySaleReportQueryHandler(AdventureWorksDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<Response<MonthlySaleReportResponse>> Handle(MonthlySaleReportQuery query)
        {
            string cacheKey = $"MonthlySaleReport-{query.Year}-{query.Month}";
            if (!_cache.TryGetValue(cacheKey, out MonthlySaleReportResponse? cachedResult))
            {
                if (query.Month < 1 || query.Month > 12)
                {
                    return ValidationError.BadRequest($"Invalid month: {query.Month}. Month must be between 1 and 12.");
                }
                if (query.Year < DateTime.MinValue.Year || query.Year > DateTime.Now.Year + 1)
                {
                    return ValidationError.BadRequest($"Invalid month: {query.Month}. Month must be between 1 and 12.");
                }
                var fromDate = new DateTime(query.Year, query.Month, 1);
                var toDate = fromDate.AddMonths(1);

                var data = await (
                    from header in _dbContext.SalesOrderHeaders
                    join detail in _dbContext.SalesOrderDetails on header.SalesOrderId equals detail.SalesOrderId
                    join product in _dbContext.Products on detail.ProductId equals product.ProductId
                    where header.OrderDate >= fromDate && header.OrderDate < toDate
                    group new { header, detail, product } by new { header.OrderDate.Year, header.OrderDate.Month } into g
                    select new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        TotalOrders = g.Select(x => x.header.SalesOrderId).Distinct().Count(),
                        TotalAmount = g.Sum(x => (decimal?)x.detail.LineTotal) ?? 0,
                        TopProduct = g
                            .GroupBy(x => new { x.product.ProductId, x.product.Name })
                            .Select(pg => new
                            {
                                ProductName = pg.Key.Name,
                                Sales = pg.Sum(x => (decimal?)x.detail.LineTotal) ?? 0
                            })
                            .OrderByDescending(pg => pg.Sales)
                            .FirstOrDefault()
                    }
                ).FirstOrDefaultAsync();

                if (data == null)
                    return ValidationError.NotFound($"No sales data for {query.Month:D2}/{query.Year}");

                var prevMonth = query.Month == 1 ? 12 : query.Month - 1;
                var prevYear = query.Month == 1 ? query.Year - 1 : query.Year;

                var prevFrom = new DateTime(prevYear, prevMonth, 1);
                var prevTo = prevFrom.AddMonths(1);

                var prevTotalAmount = await (
                    from header in _dbContext.SalesOrderHeaders
                    join detail in _dbContext.SalesOrderDetails on header.SalesOrderId equals detail.SalesOrderId
                    where header.OrderDate >= prevFrom && header.OrderDate < prevTo
                    select (decimal?)detail.LineTotal
                ).SumAsync() ?? 0;

                var growthRate = prevTotalAmount > 0
                                ? Math.Round(((data.TotalAmount - prevTotalAmount) / prevTotalAmount) * 100, 2)
                                : 0;

                var result = new MonthlySaleReportResponse
                {
                    ReportMonth = $"{data.Year}-{data.Month:D2}",
                    TotalOrders = data.TotalOrders,
                    TotalAmount = data.TotalAmount,
                    TopSaleProduct = data.TopProduct?.ProductName ?? "N/A",
                    GrowthRate = growthRate
                };

                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
                cachedResult = result;
            }

            return cachedResult;
        }
    }
}
