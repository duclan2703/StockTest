using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stock.Business.Interfaces;
using Stock.Entity;
using Stock.Entity.DTOs;
using Stock.Entity.Entities;
using Stock.Entity.Enums;

namespace Stock.Business.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly AdventureWorksDbContext _dbContext;
        private readonly IMapper _autoMapper;

        public OrderService(AdventureWorksDbContext context, IMapper autoMapper)
        {
            _dbContext = context;
            _autoMapper = autoMapper;
        }

        public async Task<bool> CreateOrderAsync(OrderDto orderDto)
        {
            if (orderDto is null)
            {
                return false;
            }

            var order = _autoMapper.Map<SalesOrderHeader>(orderDto);
            _dbContext.SalesOrderHeaders.Add(order);
            foreach (var item in order.SalesOrderDetails)
            {
                _dbContext.SalesOrderDetails.Add(item);
            }
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<SalesOrderHeader>> GetOrdersByCustomerAsync(int customerId)
        {
            var orders = await _dbContext.SalesOrderHeaders
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = _dbContext.SalesOrderHeaders.FirstOrDefault(o => o.SalesOrderId == orderId);
            if (order is null)
            {
                return false;
            }
            
            order.Status = status;
            _dbContext.SalesOrderHeaders.Update(order);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
