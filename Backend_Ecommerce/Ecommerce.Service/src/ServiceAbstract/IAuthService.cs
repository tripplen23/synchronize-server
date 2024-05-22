using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.ServiceAbstract
{
    public interface IAuthService
    {
        Task<(string token, UserRole userRole)> LoginAsync(UserCredential userCredential);
        Task<string> LogoutAsync();
        Task<UserReadDto> GetCurrentProfileAsync(Guid id);
    }
}