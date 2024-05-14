using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    public class CartService : ICartService
    {
        private IMapper _mapper;
        private readonly ICartRepo _cartRepo;
        private IProductRepo _productRepo;
        private IUserRepo _userRepo;

        public CartService(ICartRepo cartRepo, IMapper mapper, IProductRepo productRepo, IUserRepo userRepo)
        {
            _mapper = mapper;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
        }

        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            throw new NotImplementedException();
        }

        public async Task<CartReadDto> CreateCartAsync(Guid userId, CartCreateDto cartCreateDto)
        {
            var foundUser = await _userRepo.GetUserByIdAsync(userId);
            if (foundUser is null)
            {
                throw AppException.NotFound("User not found");
            }
            var existingCart = await _cartRepo.GetCartByUserIdAsync(userId);
            if (existingCart != null)
            {
                throw AppException.BadRequest("A cart already exists for this user.");
            }

            var cart = _mapper.Map<Cart>(cartCreateDto);
            cart.User = foundUser;
            var newCartItems = new List<CartItem>();
            foreach (var cartItemDto in cartCreateDto.CartItems)
            {
                var foundProduct = await _productRepo.GetProductByIdAsync(cartItemDto.ProductId);
                if (foundProduct is null)
                {
                    throw AppException.NotFound("Product not found");
                }
                newCartItems.Add(new CartItem
                {
                    Product = foundProduct,
                    Quantity = cartItemDto.Quantity
                });
            }
            cart.CartItems = newCartItems;
            var createdCart = await _cartRepo.CreateCartAsync(cart);
            var cartReadDto = _mapper.Map<CartReadDto>(createdCart);
            return cartReadDto;
        }

        public async Task<bool> DeleteCartByIdAsync(Guid cartId)
        {
            if (cartId == Guid.Empty)
            {
                throw AppException.BadRequest("Cart id is required");
            }
            try
            {
                var foundCart = await _cartRepo.GetCartByIdAsync(cartId);
                if (foundCart is not null)
                {
                    await _cartRepo.DeleteCartByIdAsync(cartId);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CartReadDto> GetCartByUserIdAsync(Guid userId)
        {
            var foundUser = _userRepo.GetUserByIdAsync(userId);
            if (foundUser is null)
            {
                throw AppException.NotFound("User not found");
            }
            var foundCart = await _cartRepo.GetCartByUserIdAsync(userId);
            if (foundCart is null)
            {
                throw AppException.NotFound($"user {userId} does not have a cart");
            }

            var cartReadDto = _mapper.Map<CartReadDto>(foundCart);
            return cartReadDto;
        }

    }
}