using System.Security.Claims;
using Ecommerce.Core.src.Common;
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
        #region Properties
        private ICartService _cartService;
        private ICartItemService _cartItemService;
        private IUserService _userService;
        private IAuthorizationService _authorizationService;
        #endregion

        #region Constructor
        public CartController(ICartService cartService, IUserService userService, ICartItemService cartItemService, IAuthorizationService authorizationService)
        {
            _cartService = cartService;
            _userService = userService;
            _cartItemService = cartItemService;
            _authorizationService = authorizationService;
        }
        #endregion

        #region POST http://localhost:5227/api/v1/carts
        [Authorize]
        [HttpPost()]
        public async Task<ActionResult<CartReadDto>> CreateCartAsync([FromBody] CartCreateDto cartCreateDto)
        {
            var userId = GetUserIdClaim();
            var result = await _cartService.CreateCartAsync(userId, cartCreateDto);

            return Ok(result);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/carts
        [Authorize(Roles = "Admin")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<CartReadDto>>> GetAllCartsAsync([FromQuery] BaseQueryOptions options)
        {
            var result = await _cartService.GetAllCartsAsync(options);

            return Ok(result);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/carts/:cartId
        // Admin and owner
        [Authorize]
        [HttpGet("{cartId:guid}")]
        public async Task<ActionResult<CartReadDto>> GetCartByIdAsync([FromRoute] Guid cartId)
        {
            CartReadDto foundCart = await _cartService.GetCartByIdAsync(cartId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, foundCart, "AdminOrOwnerCart");

            if (foundCart is null)
            {
                return NotFound();
            }

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _cartService.GetCartByIdAsync(cartId);

            return Ok(result);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/carts/user/:userId
        // Admin and owner
        [Authorize]
        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<CartReadDto>> GetCartByUserIdAsync([FromRoute] Guid userId)
        {
            var foundCart = await _cartService.GetCartByUserIdAsync(userId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, foundCart, "AdminOrOwnerCart");

            if (foundCart is null)
            {
                return NotFound();
            }

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _cartService.GetCartByUserIdAsync(userId);

            return Ok(result);
        }
        #endregion

        #region DELETE http://localhost:5227/api/v1/carts/:cartId
        // Admin or owner
        [Authorize]
        [HttpDelete("{cartId:guid}")]
        public async Task<ActionResult> DeleteCartByIdAsync([FromRoute] Guid cartId)
        {
            var foundCart = await _cartService.GetCartByIdAsync(cartId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, foundCart, "AdminOrOwnerCart");

            if (foundCart is null)
            {
                return NotFound();
            }

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            await _cartService.DeleteCartByIdAsync(cartId);
            return Ok($"Cart {cartId} deleted successfully");
        }
        #endregion

        #region PATCH http://localhost:5227/api/v1/carts/:cartId
        // Admin or owner
        [Authorize]
        [HttpPatch("{cartId}")]
        public async Task<ActionResult<CartReadDto>> UpdateCartAsync([FromRoute] Guid cartId, [FromBody] CartUpdateDto cartUpdateDto)
        {
            var foundCart = await _cartService.GetCartByIdAsync(cartId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, foundCart, "AdminOrOwnerCart");

            if (foundCart is null)
            {
                return NotFound();
            }

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _cartItemService.UpdateCartItemQuantityAsync(cartId, cartUpdateDto);
            return Ok(result);
        }
        #endregion

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