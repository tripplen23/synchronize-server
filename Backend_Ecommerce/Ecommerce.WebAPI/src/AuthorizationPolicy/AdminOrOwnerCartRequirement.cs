using System.Security.Claims;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.WebAPI.src.AuthorizationPolicy
{
    public class AdminOrOwnerCartRequirement : IAuthorizationRequirement
    {
        public AdminOrOwnerCartRequirement()
        {
        }
    }

    public class AdminOrOwnerCartHandler : AuthorizationHandler<AdminOrOwnerCartRequirement, CartReadDto>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrOwnerCartRequirement requirement, CartReadDto cart)
        {
            var claims = context.User.Claims;
            var userRole = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == cart.User.Id.ToString() || userRole == UserRole.Admin.ToString())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}