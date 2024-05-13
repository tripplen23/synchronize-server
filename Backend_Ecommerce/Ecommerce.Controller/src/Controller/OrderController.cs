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
        private IOrderService _orderService;
        private IUserService _userService;
        private IAuthorizationService _authorizationService;
        public OrderController(IOrderService orderService, IUserService userService, IAuthorizationService authorizationService)
        {
            _orderService = orderService;
            _userService = userService;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAllOrdersAsync([FromQuery] BaseQueryOptions options)
        {
            var result = await _orderService.GetAllOrdersAsync(options);
            return Ok(result);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<OrderReadDto>> GetOrdersByUserIdAsync([FromRoute] Guid UserId)
        {
            UserReadDto foundUser = await _userService.GetUserByIdAsync(UserId);
            if (foundUser is null)
            {
                return NotFound();
            }
            return Ok(await _orderService.GetOrdersByUserIdAsync(UserId)); // Will be modified later
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderReadDto>> GetOrderByIdAsync([FromRoute] Guid orderId)
        {
            var foundOrder = await _orderService.GetOrderByIdAsync(orderId);
            return Ok(foundOrder);
        }

        [HttpPost()]
        public async Task<ActionResult<OrderReadDto>> CreateOrderAsync([FromBody] OrderCreateDto orderCreateDto)
        {
            var userId = GetUserIdClaim();
            var result = await _orderService.CreateOrderAsync(userId, orderCreateDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("status/{orderId}")]
        public async Task<ActionResult<OrderReadUpdateDto>> UpdateOrderStatusAsync([FromRoute] Guid orderId, [FromBody] OrderUpdateStatusDto orderUpdateStatusDto)
        {
            orderUpdateStatusDto.OrderId = orderId;
            var result = await _orderService.UpdateOrderStatusAsync(orderId, orderUpdateStatusDto);
            return Ok(result);
        }

        [Authorize]
        [HttpPatch("{orderId}")]
        public async Task<ActionResult<OrderReadUpdateDto>> UpdateOrderQuantityAsync(Guid orderId, [FromBody] OrderUpdateDto orderUpdateDto)
        {
            var foundOrder = await _orderService.GetOrderByIdAsync(orderId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, foundOrder, "AdminOrOwnerOrder");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _orderService.UpdateOrderQuantityAsync(orderId, orderUpdateDto);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{orderId}/products/{productId}")]
        public async Task<ActionResult<OrderReadUpdateDto>> DeleteProductFromOrderAsync(Guid orderId, Guid productId)
        {
            var foundOrder = await _orderService.GetOrderByIdAsync(orderId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, foundOrder, "AdminOrOwnerOrder");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _orderService.DeleteProductFromOrderAsync(orderId, productId);

            return Ok(result);
        }




        [Authorize]
        [HttpDelete("{orderId}")]
        public async Task<ActionResult<bool>> DeleteAnOrderByIdAsync([FromRoute] Guid orderId)
        {
            OrderReadDto? foundOrder = await _orderService.GetOrderByIdAsync(orderId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, foundOrder, "AdminOrOwnerOrder");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _orderService.DeleteOrderByIdAsync(orderId);

            return Ok(result);
        }

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

        // Update order product information (Quantity, productId, ...) -> Will implement later.
        // UpdateOrderStatusAsync(OrderId, OrderUpdateDto orderUpdate);
    }
}