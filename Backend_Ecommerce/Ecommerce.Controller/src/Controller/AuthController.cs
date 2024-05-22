using System.Security.Claims;
using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        #region Properties
        private readonly IAuthService _authService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserService _userService;
        #endregion

        #region Constructor
        public AuthController(IAuthService authService, IAuthorizationService authorizationService, IUserService userService)
        {
            _authService = authService;
            _authorizationService = authorizationService;
            _userService = userService;
        }
        #endregion

        #region POST http://localhost:5227/api/v1/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<object>> LoginAsync([FromBody] UserCredential userCredential)
        {
            var (token, userRole) = await _authService.LoginAsync(userCredential);
            return Ok(new { token, userRole });
        }
        #endregion

        #region GET http://localhost:5227/api/v1/auth/profile
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserReadDto>> GetCurrnentProfileAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await _userService.GetUserByIdAsync(Guid.Parse(userId));
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, user, "AdminOrOwnerAccount");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            return await _authService.GetCurrentProfileAsync(user.Id);
        }
        #endregion

        #region POST http://localhost:5227/api/v1/auth/logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            // Retrieve token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Check if the token exists
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            // Call the logout method
            var result = await _authService.LogoutAsync();
            if (result == "removed")
            {
                return Ok("Logged out successfully");
            }
            else
            {
                return BadRequest("User is already logout");
            }
        }
        #endregion
    }
}