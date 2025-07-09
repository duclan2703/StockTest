using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Stock.Business.Implementation;
using Stock.Entity;
using Stock.Entity.DTOs;
using Stock.Entity.Entities;
using Stock.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    public class ServiceTest
    {
        private readonly AdventureWorksDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly OrderService _service;

        public ServiceTest()
        {
            // Substitute DbContext and IMapper
            var options = new DbContextOptionsBuilder<AdventureWorksDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = Substitute.ForPartsOf<AdventureWorksDbContext>(options);
            _mapper = Substitute.For<IMapper>();
            _service = new OrderService(_dbContext, _mapper);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsFalse_WhenOrderDtoIsNull()
        {
            var result = await _service.CreateOrderAsync(null);
            Assert.False(result);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsTrue_WhenOrderIsCreated()
        {
            // Arrange
            var orderDto = new OrderDto
            {
                SalesOrderNumber = "SO12345",
                OrderDetails = new List<OrderDetailDto>
                {
                    new OrderDetailDto { SalesOrderDetailId = 1, ProductId = 1, OrderQty = 1 }
                }
            };
            var salesOrder = new SalesOrderHeader
            {
                SalesOrderNumber = "SO12345",
                SalesOrderDetails = new List<SalesOrderDetail>
                {
                    new SalesOrderDetail { SalesOrderDetailId = 1, ProductId = 1, OrderQty = 1 }
                }
            };
            _mapper.Map<SalesOrderHeader>(orderDto).Returns(salesOrder);

            // Act
            var result = await _service.CreateOrderAsync(orderDto);

            // Assert
            Assert.True(result);
            Assert.Single(_dbContext.SalesOrderHeaders);
            Assert.Single(_dbContext.SalesOrderDetails);
        }

        [Fact]
        public async Task GetOrdersByCustomerAsync_ReturnsOrders()
        {
            // Arrange
            var customerId = 42;
            _dbContext.SalesOrderHeaders.Add(new SalesOrderHeader { CustomerId = customerId, SalesOrderNumber = "SO12345" });
            _dbContext.SalesOrderHeaders.Add(new SalesOrderHeader { CustomerId = 99, SalesOrderNumber = "SO12345" });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetOrdersByCustomerAsync(customerId);

            // Assert
            Assert.Single(result);
            Assert.All(result, o => Assert.Equal(customerId, o.CustomerId));
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ReturnsFalse_WhenOrderNotFound()
        {
            // Act
            var result = await _service.UpdateOrderStatusAsync(999, OrderStatus.Approved);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_UpdatesStatus_WhenOrderExists()
        {
            // Arrange
            var order = new SalesOrderHeader { SalesOrderId = 12345, Status = OrderStatus.InProcess, SalesOrderNumber = "SO12345" };
            _dbContext.SalesOrderHeaders.Add(order);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.UpdateOrderStatusAsync(12345, OrderStatus.Approved);

            // Assert
            Assert.True(result);
            Assert.Equal(OrderStatus.Approved, order.Status);
        }
    }
}