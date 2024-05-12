using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepo _userRepo;
        private ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private IMapper _mapper;
        public AuthService(IUserRepo userRepo, ITokenService tokenService, IPasswordService passwordService, IMapper mapper)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task<string> LoginAsync(UserCredential userCredential)
        {
            var foundUser = await _userRepo.GetUserByEmailAsync(userCredential.Email) ?? throw AppException.NotFound("Email is not registered");

            var isMatch = _passwordService.VerifyPassword(userCredential.Password, foundUser.Password, foundUser.Salt);
            if (isMatch)
            {
                return _tokenService.GetToken(foundUser);
            }
            else
            {
                throw AppException.InvalidLoginCredentialsException("Incorrect password");
            }
        }

        public async Task<UserReadDto> GetCurrentProfileAsync(Guid id)
        {
            var foundUser = await _userRepo.GetUserByIdAsync(id);
            if (foundUser != null)
            {
                return _mapper.Map<User, UserReadDto>(foundUser);
            }
            throw AppException.NotFound("User not found");
        }

        public async Task<string> LogoutAsync()
        {
            return await _tokenService.InvalidateTokenAsync();
        }
    }
}