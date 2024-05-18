using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.Service;
using Ecommerce.Service.src.ServiceAbstract;
using Moq;
using Xunit;

namespace Ecommerce.Test.src.Service
{
    public class AuthServiceTest
    {
        private readonly AuthService _authService;
        private readonly Mock<IUserRepo> _userRepoMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IPasswordService> _passwordServiceMock = new();
        private Mock<IMapper> _mapperMock = new();

        public AuthServiceTest()
        {
            _authService = new AuthService(_userRepoMock.Object, _tokenServiceMock.Object, _passwordServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var userCredential = new UserCredential { Email = "test@example.com", Password = "password" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                Password = "hashedpassword",
                Salt = new byte[] { 1, 2, 3, 4 },
                UserRole = UserRole.Customer
            };
            var token = "dummytoken";

            _userRepoMock.Setup(repo => repo.GetUserByEmailAsync(userCredential.Email))
                         .ReturnsAsync(user);

            _passwordServiceMock.Setup(service => service.VerifyPassword(userCredential.Password, user.Password, user.Salt))
                                .Returns(true);

            _tokenServiceMock.Setup(service => service.GetToken(user))
                             .Returns(token);

            // Act
            var result = await _authService.LoginAsync(userCredential);

            // Assert
            Assert.Equal(token, result);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ThrowsException()
        {
            // Arrange
            var userCredential = new UserCredential { Email = "test@example.com", Password = "wrongpassword" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                Password = "hashedpassword",
                Salt = new byte[] { 1, 2, 3, 4 },
                UserRole = UserRole.Customer
            };

            _userRepoMock.Setup(repo => repo.GetUserByEmailAsync(userCredential.Email))
                         .ReturnsAsync(user);

            _passwordServiceMock.Setup(service => service.VerifyPassword(userCredential.Password, user.Password, user.Salt))
                                .Returns(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _authService.LoginAsync(userCredential));
            Assert.Equal("Incorrect password", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_EmailNotRegistered_ThrowsException()
        {
            // Arrange
            var userCredential = new UserCredential { Email = "notregistered@example.com", Password = "password" };

            _userRepoMock.Setup(repo => repo.GetUserByEmailAsync(userCredential.Email))
                         .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _authService.LoginAsync(userCredential));
            Assert.Equal($"Email - {userCredential.Email} is not registered", exception.Message);
        }

        [Fact]
        public async Task GetCurrentProfileAsync_ValidUserId_ReturnsUserReadDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User", Email = "test@example.com", UserRole = UserRole.Customer };
            var userReadDto = new UserReadDto { Id = userId, UserName = "Test User", UserEmail = "test@example.com" };

            _userRepoMock.Setup(repo => repo.GetUserByIdAsync(userId))
                        .ReturnsAsync(user);

            _mapperMock.Setup(mapper => mapper.Map<User, UserReadDto>(user))
                       .Returns(userReadDto);

            // Act
            var result = await _authService.GetCurrentProfileAsync(userId);

            // Assert
            Assert.Equal(userReadDto, result);
        }

        [Fact]
        public async Task GetCurrentProfileAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepoMock.Setup(repo => repo.GetUserByIdAsync(userId))
                         .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _authService.GetCurrentProfileAsync(userId));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task LogoutAsync_ValidRequest_ReturnsSuccessMessage()
        {
            // Arrange
            var successMessage = "removed";

            _tokenServiceMock.Setup(service => service.InvalidateTokenAsync())
                             .ReturnsAsync(successMessage);

            // Act
            var result = await _authService.LogoutAsync();

            // Assert
            Assert.Equal(successMessage, result);
        }

        [Fact]
        public async Task LogoutAsync_TokenAlreadyRemoved_ReturnsAlreadyRemovedMessage()
        {
            // Arrange
            var alreadyRemovedMessage = "already removed";

            _tokenServiceMock.Setup(service => service.InvalidateTokenAsync())
                             .ReturnsAsync(alreadyRemovedMessage);

            // Act
            var result = await _authService.LogoutAsync();

            // Assert
            Assert.Equal(alreadyRemovedMessage, result);
        }
    }
}