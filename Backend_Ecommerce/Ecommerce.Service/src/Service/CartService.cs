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
        private readonly ICartItemRepo _cartItemRepo;
        private IProductRepo _productRepo;
        private IUserRepo _userRepo;

        public CartService(ICartRepo cartRepo, IMapper mapper, IProductRepo productRepo, IUserRepo userRepo, ICartItemRepo cartItemRepo)
        {
            _mapper = mapper;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
            _cartItemRepo = cartItemRepo;
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
                // Update existing cart
                return await UpdateCartItems(existingCart.Id, cartCreateDto.CartItems);
            }

            // Create new cart
            var cart = _mapper.Map<Cart>(cartCreateDto);
            cart.User = foundUser;
            var newCartItems = new List<CartItem>();
            foreach (var cartItemDto in cartCreateDto.CartItems)
            {
                // Quantity must be positive
                if (cartItemDto.Quantity < 0)
                {
                    throw AppException.BadRequest("Quantity must be positive");
                }

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
            // Remove cart items with quantity 0
            var itemsToRemove = newCartItems.Where(ci => ci.Quantity == 0).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                await _cartItemRepo.DeleteCartItemByIdAsync(itemToRemove.Id);
                newCartItems.Remove(itemToRemove);
            }

            cart.CartItems = newCartItems;
            var createdCart = await _cartRepo.CreateCartAsync(cart);
            var cartReadDto = _mapper.Map<CartReadDto>(createdCart);
            return cartReadDto;
        }

        private async Task<CartReadDto> UpdateCartItems(Guid cartId, IEnumerable<CartItemCreateDto> cartItemsDto)
        {
            var foundCart = await _cartRepo.GetCartByIdAsync(cartId);
            if (foundCart == null)
            {
                throw AppException.NotFound("Cart not found");
            }

            var cartItemsList = foundCart.CartItems.ToList();
            foreach (var cartItemDto in cartItemsDto)
            {
                var existingCartItem = cartItemsList.FirstOrDefault(ci => ci.ProductId == cartItemDto.ProductId);
                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += cartItemDto.Quantity;
                }
                else
                {
                    var foundProduct = await _productRepo.GetProductByIdAsync(cartItemDto.ProductId);
                    if (foundProduct == null)
                    {
                        throw AppException.NotFound("Product not found");
                    }
                    cartItemsList.Add(new CartItem
                    {
                        CartId = cartId,
                        ProductId = cartItemDto.ProductId,
                        Quantity = cartItemDto.Quantity
                    });
                }
            }
            // Remove cart items with quantity 0
            var itemsToRemove = cartItemsList.Where(ci => ci.Quantity == 0).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                await _cartItemRepo.DeleteCartItemByIdAsync(itemToRemove.Id);
                cartItemsList.Remove(itemToRemove);
            }

            foundCart.CartItems = cartItemsList;
            var updatedCart = await _cartRepo.UpdateCartAsync(foundCart);
            var cartReadDto = _mapper.Map<CartReadDto>(updatedCart);
            return cartReadDto;
        }

        public async Task<IEnumerable<CartReadDto>> GetAllCartsAsync(BaseQueryOptions options)
        {
            var cartList = await _cartRepo.GetAllCartsAsync(options);
            var cartDtos = new List<CartReadDto>();

            foreach (var cart in cartList)
            {
                var cartDto = _mapper.Map<CartReadDto>(cart);
                cartDtos.Add(cartDto);
            }

            return cartDtos;
        }

        public async Task<CartReadDto> GetCartByIdAsync(Guid cartId)
        {
            if (cartId == Guid.Empty)
            {
                throw AppException.BadRequest("CartId is required");
            }
            try
            {
                var foundCart = await _cartRepo.GetCartByIdAsync(cartId);
                if (foundCart is null)
                {
                    throw AppException.NotFound("Cart not found");
                }
                var cartDto = _mapper.Map<CartReadDto>(foundCart);
                var user = await _userRepo.GetUserByIdAsync(foundCart.UserId);
                cartDto.User = _mapper.Map<UserReadDto>(user);
                cartDto.CartItems = _mapper.Map<IEnumerable<CartItemReadDto>>(cartDto.CartItems);
                return cartDto;
            }
            catch (Exception)
            {
                throw;
            }
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
    }
}