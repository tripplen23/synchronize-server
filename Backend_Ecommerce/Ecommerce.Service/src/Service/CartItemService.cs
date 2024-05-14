using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    public class CartItemService : ICartItemService
    {
        private IMapper _mapper;
        private readonly ICartRepo _cartRepo;
        private readonly IProductRepo _productRepo;
        private readonly IUserRepo _userRepo;

        public CartItemService(ICartRepo cartRepo, IMapper mapper, IProductRepo productRepo, IUserRepo userRepo)
        {
            _mapper = mapper;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
        }

        public async Task<CartReadDto> UpdateCartItemQuantityAsync(Guid cartId, CartUpdateDto cartUpdateDto)
        {
            var foundCart = await _cartRepo.GetCartByIdAsync(cartId);
            if (foundCart is null)
            {
                throw AppException.NotFound("Cart not found");
            }

            foundCart.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            // Convert CartItem to a list
            var cartItemsList = foundCart.CartItems.ToList();

            foreach (var updatedCartItem in cartUpdateDto.CartItems)
            {
                var foundCartItem = cartItemsList.FirstOrDefault(ci => ci.ProductId == updatedCartItem.ProductId);
                if (foundCartItem is not null)
                {
                    foundCartItem.Quantity = updatedCartItem.Quantity;
                }
                else
                {
                    // If the product is not available in the cart, search it in the database, if it exists, add it to the cart, otherwise throw an exception
                    var foundProduct = await _productRepo.GetProductByIdAsync(updatedCartItem.ProductId);
                    if (foundProduct is null)
                    {
                        throw AppException.NotFound("Product not found");
                    }
                    cartItemsList.Add(
                        new CartItem
                        {
                            CartId = cartId,
                            ProductId = foundProduct.Id,
                            Quantity = updatedCartItem.Quantity
                        }
                    );
                }
            }

            foundCart.CartItems = cartItemsList;

            var updatedCart = await _cartRepo.UpdateCartAsync(foundCart);

            var user = await _userRepo.GetUserByIdAsync(updatedCart.UserId);
            var cartReadDto = _mapper.Map<CartReadDto>(updatedCart);
            cartReadDto.User = _mapper.Map<UserReadDto>(user);
            cartReadDto.CartItems = _mapper.Map<IEnumerable<CartItemReadDto>>(updatedCart.CartItems);

            return cartReadDto;
        }
    }
}