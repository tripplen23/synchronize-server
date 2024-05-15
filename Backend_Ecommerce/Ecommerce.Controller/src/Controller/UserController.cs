using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        #region Properties
        private readonly IUserService _userService;
        private IAuthorizationService _authorizationService;
        #endregion

        #region Constructors
        public UserController(IUserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }
        #endregion

        #region GET http://localhost:5227/api/v1/users
        [Authorize(Roles = "Admin")]
        [HttpGet] // endpoint: /users
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllUsersAsync([FromQuery] UserQueryOptions userQueryOptions)
        {
            var result = await _userService.GetAllUsersAsync(userQueryOptions);
            return Ok(result);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/users/{userId}
        [Authorize(Roles = "Admin")]
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserReadDto>> GetUserByIdAsync([FromRoute] Guid userId)
        {
            var foundUser = await _userService.GetUserByIdAsync(userId);
            if (foundUser is null)
            {
                return NotFound();
            }
            return Ok(foundUser);
        }
        #endregion

        #region POST http://localhost:5227/api/v1/users
        [HttpPost()]
        public async Task<ActionResult<UserReadDto>> CreateUserAsync([FromBody] UserCreateDto userCreateDto)
        {
            var createdUser = await _userService.CreateUserAsync(userCreateDto);
            return Ok(createdUser);
        }
        #endregion

        #region PUT http://localhost:5227/api/v1/users/{userId}
        // Admin and owner
        [Authorize]
        [HttpPut("{userId}")]
        public async Task<ActionResult<UserReadDto>> UpdateUserByIdAsync([FromRoute] Guid userId, [FromBody] UserUpdateDto userUpdateDto)
        {
            var foundUser = await _userService.GetUserByIdAsync(userId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, foundUser, "AdminOrOwnerAccount");
            if (foundUser == null)
            {
                return NotFound();
            }
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            var updatedUser = await _userService.UpdateUserByIdAsync(userId, userUpdateDto);
            return Ok(updatedUser);
        }
        #endregion

        #region DELETE http://localhost:5227/api/v1/users/{userId}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")] // endpoint: /users/:user_id
        public async Task<IActionResult> DeleteUserByIdAsync([FromRoute] Guid userId)
        {
            var isDeleted = await _userService.DeleteUserByIdAsync(userId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
        #endregion
    }
}
