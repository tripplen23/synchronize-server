using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.WebAPI.src.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repo
{
    public class CartItemRepo : ICartItemRepo
    {
        private readonly AppDbContext _context;
        private readonly DbSet<CartItem> _cartItems;

        public CartItemRepo(AppDbContext context)
        {
            _context = context;
            _cartItems = _context.CartItems;
        }

        public async Task<bool> DeleteCartItemByIdAsync(Guid cartItemId)
        {
            var cartItem = await _cartItems.FirstOrDefaultAsync(x => x.Id == cartItemId);
            if (cartItem != null)
            {
                _context.Remove(cartItem);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
