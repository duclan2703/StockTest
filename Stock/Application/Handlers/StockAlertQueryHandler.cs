using Application.Requests.Queries;
using Application.Responses;
using Core;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Stock.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers
{
    public class StockAlertQueryHandler : IQueryHandler<StockAlertQuery, Response<List<StockAlertResponse>>>
    {
        private readonly AdventureWorksDbContext _dbContext;

        public StockAlertQueryHandler(AdventureWorksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Response<List<StockAlertResponse>>> Handle(StockAlertQuery query)
        {
            var threeMonthsAgo = query.RequestDate.AddMonths(-3);
            var stockData = await _dbContext.ProductInventories
                    .GroupBy(pi => pi.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        StockQty = g.Sum(pi => pi.Quantity)
                    })
                    .ToListAsync();

            var salesData = await _dbContext.SalesOrderDetails
                .Where(sod => sod.SalesOrderHeader.OrderDate >= threeMonthsAgo)
                .GroupBy(sod => sod.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(sod => sod.OrderQty),
                    AvgSold = g.Average(sod => sod.OrderQty)
                })
                .ToListAsync();

            var products = await _dbContext.Products
                .Where(p => p.FinishedGoodsFlag == true)
                .Select(p => new
                {
                    p.ProductId,
                    ProductName = p.Name
                })
                .ToListAsync();

            var result = (from p in products
                                  join s in stockData on p.ProductId equals s.ProductId into stockGroup
                                  from stock in stockGroup.DefaultIfEmpty()
                                  join sal in salesData on p.ProductId equals sal.ProductId into salesGroup
                                  from sales in salesGroup.DefaultIfEmpty()
                                  where sales != null && sales.TotalSold > 0 &&
                                        (stock?.StockQty ?? 0) < (2 * sales.AvgSold)
                                  select new StockAlertResponse
                                  {
                                      ProductId = p.ProductId,
                                      ProductName = p.ProductName,
                                      StockQtty = stock?.StockQty ?? 0,
                                      AvgSoldLast3Months = Math.Round(sales.AvgSold, 2),
                                      ExpectedShortage = Math.Round((2 * sales.AvgSold) - (stock?.StockQty ?? 0), 2)
                                  })
                    .OrderByDescending(x => x.ExpectedShortage)
                    .ToList();

            return result;
        }
    }
}
