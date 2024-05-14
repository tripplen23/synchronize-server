using System.Security.Claims;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/carts")]
    public class CartController : ControllerBase
    {
        private ICartService _cartService;
        private ICartItemService _cartItemService;
        private IUserService _userService;

        public CartController(ICartService cartService, IUserService userService, ICartItemService cartItemService)
        {
            _cartService = cartService;
            _userService = userService;
            _cartItemService = cartItemService;
        }

        [Authorize]
        [HttpPost()]
        public async Task<ActionResult<CartReadDto>> CreateCartAsync([FromBody] CartCreateDto cartCreateDto)
        {
            var userId = GetUserIdClaim();
            var result = await _cartService.CreateCartAsync(userId, cartCreateDto);
            return Ok(result);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<CartReadDto>> GetCartByUserIdAsync([FromRoute] Guid userId)
        {
            UserReadDto foundUser = await _userService.GetUserByIdAsync(userId);
            if (foundUser is null)
            {
                return NotFound();
            }
            var result = await _cartService.GetCartByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpDelete("{cartId}")]
        public async Task<ActionResult> DeleteCartByIdAsync([FromRoute] Guid cartId)
        {
            var result = await _cartService.DeleteCartByIdAsync(cartId);
            if (!result)
            {
                return NotFound();
            }
            return Ok($"Cart {cartId} deleted successfully");
        }

        [HttpDelete("{cartId}/clear")]
        public async Task<ActionResult> ClearCartAsync([FromRoute] Guid cartId)
        {
            var result = await _cartService.ClearCartAsync(cartId);
            if (!result)
            {
                return NotFound();
            }
            return Ok($"Cart {cartId} cleared successfully");
        }

        [Authorize]
        [HttpPatch("{cartId}")]
        public async Task<ActionResult<CartReadDto>> UpdateCartAsync([FromRoute] Guid cartId, [FromBody] CartUpdateDto cartUpdateDto)
        {
            var result = await _cartItemService.UpdateCartItemQuantityAsync(cartId, cartUpdateDto);
            return Ok(result);
        }


        #region Helper Methods
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