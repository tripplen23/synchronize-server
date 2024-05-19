using System.Security.Claims;
using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/reviews")]
    public class ReviewController : ControllerBase
    {
        #region Properties
        private readonly IReviewService _service;
        private readonly IAuthorizationService _authorizationService;
        #endregion

        #region Constructor
        public ReviewController(IReviewService service, IAuthorizationService authorizationService)
        {
            _service = service;
            _authorizationService = authorizationService;
        }
        #endregion

        #region GET http://localhost:5227/api/v1/reviews
        [AllowAnonymous]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetAllReviewsAsync([FromQuery] BaseQueryOptions options)
        {
            var reviews = await _service.GetAllReviewsAsync(options);
            return Ok(reviews);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/reviews/product/{productId}
        [AllowAnonymous]
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetAllReviewsOfProductAsync([FromQuery] Guid productId)
        {
            var reviews = await _service.GetAllReviewsOfProductAsync(productId);
            return Ok(reviews);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/reviews/{reviewId}
        [Authorize]
        [HttpGet("{reviewId}")]
        public async Task<ActionResult<ReviewReadDto>> GetReviewByIdAsync([FromRoute] Guid reviewId)
        {
            var review = await _service.GetReviewByIdAsync(reviewId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, review, "AdminOrOwnerReview");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }
        #endregion

        #region POST http://localhost:5227/api/v1/reviews
        [Authorize]
        [HttpPost()]
        public async Task<ActionResult<ReviewReadDto>> CreateReviewAsync([FromBody] ReviewCreateDto reviewCreateDto)
        {
            var userId = GetUserIdClaim();
            return Ok(await _service.CreateReviewAsync(userId, reviewCreateDto));
        }
        #endregion

        #region PATCH http://localhost:5227/api/v1/reviews/{reviewId}
        [Authorize]
        [HttpPatch("{reviewId}")]
        public async Task<ActionResult<ReviewReadDto>> UpdateReviewByIdAsync([FromRoute] Guid reviewId, [FromBody] ReviewUpdateDto reviewUpdateDto)
        {
            reviewUpdateDto.ReviewId = reviewId;
            var review = await _service.GetReviewByIdAsync(reviewId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, review, "AdminOrOwnerReview");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(await _service.UpdateReviewByIdAsync(reviewId, reviewUpdateDto));
        }
        #endregion

        #region DELETE http://localhost:5227/api/v1/reviews/{reviewId}
        [Authorize]
        [HttpDelete("{reviewId}")]
        public async Task<ActionResult<bool>> DeleteReviewByIdAsync([FromRoute] Guid reviewId)
        {
            var review = await _service.GetReviewByIdAsync(reviewId);
            var authResult = await _authorizationService.AuthorizeAsync(HttpContext.User, review, "AdminOrOwnerReview");

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(await _service.DeleteReviewByIdAsync(reviewId));
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