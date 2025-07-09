using Stock.Entity.DTOs;
using Stock.Entity.Entities;
using Stock.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Business.Interfaces
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(OrderDto orderDto);
        Task<IEnumerable<SalesOrderHeader>> GetOrdersByCustomerAsync(int customerId);
        Task<bool> UpdateOrderStatusAsync(int orderID, OrderStatus status);
    }
}
