using System.Security.Claims;
using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrderController : ControllerBase
    {
        #region Properties
        private IOrderService _orderService;
        private IUserService _userService;
        private IAuthorizationService _authorizationService;
        #endregion

        #region Constructor
        public OrderController(IOrderService orderService, IUserService userService, IAuthorizationService authorizationService)
        {
            _orderService = orderService;
            _userService = userService;
            _authorizationService = authorizationService;
        }
        #endregion

        #region GET http://localhost:5227/api/v1/orders
        [Authorize(Roles = "Admin")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAllOrdersAsync([FromQuery] BaseQueryOptions options)
        {
            var result = await _orderService.GetAllOrdersAsync(options);
            return Ok(result);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/orders/user/:userId
        [AllowAnonymous]
        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<OrderReadDto>> GetOrdersByUserIdAsync([FromRoute] Guid userId)
        {
            UserReadDto foundUser = await _userService.GetUserByIdAsync(userId);
            if (foundUser is null)
            {
                return NotFound();
            }
            var result = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(result);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/orders/:orderId
        [AllowAnonymous]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderReadDto>> GetOrderByIdAsync([FromRoute] Guid orderId)
        {
            var foundOrder = await _orderService.GetOrderByIdAsync(orderId);
            return Ok(foundOrder);
        }
        #endregion

        #region POST http://localhost:5227/api/v1/orders
        [Authorize]
        [HttpPost()]
        public async Task<ActionResult<OrderReadDto>> CreateOrderAsync([FromBody] OrderCreateDto orderCreateDto)
        {
            var userId = GetUserIdClaim();
            var result = await _orderService.CreateOrderAsync(userId, orderCreateDto);
            return Ok(result);
        }
        #endregion

        #region PATCH http://localhost:5227/api/v1/orders/:orderId
        [Authorize(Roles = "Admin")]
        [HttpPatch("{orderId}")]
        public async Task<ActionResult<OrderReadUpdateDto>> UpdateOrderStatusAsync([FromRoute] Guid orderId, [FromBody] OrderUpdateStatusDto orderUpdateStatusDto)
        {
            orderUpdateStatusDto.OrderId = orderId;
            var result = await _orderService.UpdateOrderStatusAsync(orderId, orderUpdateStatusDto);
            return Ok(result);
        }
        #endregion

        #region DELETE http://localhost:5227/api/v1/orders/:orderId
        [Authorize(Roles = "Admin")]
        [HttpDelete("{orderId}")]
        public async Task<ActionResult<bool>> DeleteAnOrderByIdAsync([FromRoute] Guid orderId)
        {
            var foundOrder = await _orderService.GetOrderByIdAsync(orderId);
            if (foundOrder is null)
            {
                return NotFound("Order not found");
            }
            var result = await _orderService.DeleteOrderByIdAsync(orderId);

            return Ok(result);
        }
        #endregion

        #region Helper methods
        private Guid GetUserIdClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("User ID claim not found");
            }
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("Invalid user ID format");
            }
            return userId;
        }
        #endregion
    }
}