using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.Service;
using Ecommerce.Service.src.Shared;
using Moq;
using Xunit;

namespace Ecommerce.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepo> _orderRepoMock;
        private readonly Mock<IProductRepo> _productRepoMock;
        private readonly Mock<IUserRepo> _userRepoMock;
        private readonly IMapper _mapper;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepoMock = new Mock<IOrderRepo>();
            _productRepoMock = new Mock<IProductRepo>();
            _userRepoMock = new Mock<IUserRepo>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
            _mapper = config.CreateMapper();

            _orderService = new OrderService(_orderRepoMock.Object, _mapper, _productRepoMock.Object, _userRepoMock.Object);
        }

        #region CREATE
        [Fact]
        public async Task CreateOrderAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderCreateDto = new OrderCreateDto();

            _userRepoMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _orderService.CreateOrderAsync(userId, orderCreateDto));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_ProductNotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderCreateDto = new OrderCreateDto
            {
                OrderProducts = new List<OrderProductCreateDto> { new OrderProductCreateDto { ProductId = Guid.NewGuid(), Quantity = 1 } }
            };
            var user = new User { Id = userId };

            _userRepoMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _orderService.CreateOrderAsync(userId, orderCreateDto));
            Assert.Equal("Product not found", exception.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidOrder_ReturnsOrderReadDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderCreateDto = new OrderCreateDto
            {
                OrderProducts = new List<OrderProductCreateDto> {
                    new OrderProductCreateDto
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 1
                    }
                }
            };
            var user = new User { Id = userId, Name = "Test User", Email = "test@example.com" };
            var product = new Product { Id = orderCreateDto.OrderProducts.First().ProductId, Price = 100 };
            var order = new Order { Id = Guid.NewGuid(), User = user, OrderProducts = new List<OrderProduct>(), TotalPrice = 100 };
            var createdOrder = new Order { Id = Guid.NewGuid(), User = user, OrderProducts = order.OrderProducts, TotalPrice = 100 };

            _userRepoMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync(product);
            _orderRepoMock.Setup(repo => repo.CreateOrderAsync(It.IsAny<Order>())).ReturnsAsync(createdOrder);

            // Act
            var result = await _orderService.CreateOrderAsync(userId, orderCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdOrder.Id, result.Id);
            Assert.Equal(100, result.TotalPrice);
        }
        #endregion

        #region GET
        [Fact]
        public async Task GetOrderByIdAsync_OrderNotFound_ThrowsException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderRepoMock.Setup(repo => repo.GetOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _orderService.GetOrderByIdAsync(orderId));
            Assert.Equal("Order not found", exception.Message);
        }

        [Fact]
        public async Task GetOrderByIdAsync_OrderFound_ReturnsOrderReadDto()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var order = new Order { Id = orderId, UserId = userId, User = new User { Id = userId }, OrderProducts = new List<OrderProduct>() };

            _orderRepoMock.Setup(repo => repo.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            _userRepoMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(new User { Id = userId });

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
        }
        #endregion

        #region DELETE
        [Fact]
        public async Task DeleteOrderByIdAsync_OrderFound_ReturnsTrue()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { Id = orderId };

            _orderRepoMock.Setup(repo => repo.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            _orderRepoMock.Setup(repo => repo.DeleteOrderByIdAsync(orderId)).ReturnsAsync(true);

            // Act
            var result = await _orderService.DeleteOrderByIdAsync(orderId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteOrderByIdAsync_OrderNotFound_ReturnsFalse()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderRepoMock.Setup(repo => repo.GetOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

            // Act
            var result = await _orderService.DeleteOrderByIdAsync(orderId);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region PATCH
        [Fact]
        public async Task UpdateOrderStatusAsync_OrderNotFound_ThrowsException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderUpdateStatusDto = new OrderUpdateStatusDto { OrderStatus = OrderStatus.Shipped };

            _orderRepoMock.Setup(repo => repo.GetOrderByIdAsync(orderId)).ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _orderService.UpdateOrderStatusAsync(orderId, orderUpdateStatusDto));
            Assert.Equal("Order not found", exception.Message);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ValidOrder_ReturnsUpdatedOrderReadDto()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderUpdateStatusDto = new OrderUpdateStatusDto { OrderStatus = OrderStatus.Shipped };
            var order = new Order { Id = orderId, Status = OrderStatus.Pending, UserId = Guid.NewGuid(), OrderProducts = new List<OrderProduct>() };
            var updatedOrder = new Order { Id = orderId, Status = OrderStatus.Shipped, UserId = order.UserId, OrderProducts = order.OrderProducts };

            _orderRepoMock.Setup(repo => repo.GetOrderByIdAsync(orderId)).ReturnsAsync(order);
            _orderRepoMock.Setup(repo => repo.UpdateOrderAsync(It.IsAny<Order>())).ReturnsAsync(updatedOrder);
            _userRepoMock.Setup(repo => repo.GetUserByIdAsync(order.UserId)).ReturnsAsync(new User { Id = order.UserId });

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(orderId, orderUpdateStatusDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(OrderStatus.Shipped, result.OrderStatus);
        }
        #endregion
    }
}