using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    public class AuthService : IAuthService
    {
        #region Fields
        private readonly IUserRepo _userRepo;
        private ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private IMapper _mapper;
        #endregion

        #region constructor
        public AuthService(IUserRepo userRepo, ITokenService tokenService, IPasswordService passwordService, IMapper mapper)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _mapper = mapper;
        }
        #endregion

        #region Login
        public async Task<(string token, UserRole userRole)> LoginAsync(UserCredential userCredential)
        {
            var foundUser = await _userRepo.GetUserByEmailAsync(userCredential.Email) ?? throw AppException.NotFound($"Email - {userCredential.Email} is not registered");

            var isMatch = _passwordService.VerifyPassword(userCredential.Password, foundUser.Password, foundUser.Salt);
            if (isMatch)
            {
                var token = _tokenService.GetToken(foundUser);
                return (token, foundUser.UserRole);
            }
            else
            {
                throw AppException.InvalidLoginCredentialsException("Incorrect password");
            }
        }
        #endregion

        #region Get User Profile
        public async Task<UserReadDto> GetCurrentProfileAsync(Guid id)
        {
            var foundUser = await _userRepo.GetUserByIdAsync(id);
            if (foundUser != null)
            {
                return _mapper.Map<User, UserReadDto>(foundUser);
            }
            throw AppException.NotFound("User not found");
        }
        #endregion

        #region Logout
        public async Task<string> LogoutAsync()
        {
            return await _tokenService.InvalidateTokenAsync();
        }
        #endregion
    }
}