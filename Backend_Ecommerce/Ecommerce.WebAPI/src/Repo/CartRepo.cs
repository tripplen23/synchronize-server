using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.WebAPI.src.Database;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.WebAPI.src.Repo
{
    public class CartRepo : ICartRepo
    {

        private readonly AppDbContext _context;
        private readonly DbSet<Cart> _carts;
        private DbSet<Product> _products;
        private readonly DbSet<OrderProduct> _orderProducts;

        public CartRepo(AppDbContext context)
        {
            _context = context;
            _carts = _context.Carts;
            _products = _context.Products;
            _orderProducts = _context.OrderProducts;
        }
        public async Task<Cart> CreateCartAsync(Cart createdCart)
        {
            await _context.Carts.AddAsync(createdCart);
            await _context.SaveChangesAsync();
            return createdCart;
        }

        public async Task<bool> DeleteCartByIdAsync(Guid cartId)
        {
            var foundCart = await _context.Carts.FindAsync(cartId);
            if (foundCart != null)
            {
                _context.Carts.Remove(foundCart);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Cart>> GetAllCartsAsync(BaseQueryOptions options)
        {
            var query = _context.Carts.AsQueryable();
            if (options != null)
            {
                query = query.Skip(options.Offset).Take(options.Limit);
            }
            return await query.ToListAsync();
        }

        public async Task<Cart> GetCartByIdAsync(Guid cartId)
        {
            var query = _carts.AsQueryable();
            query = query.Include(c => c.CartItems)
                         .ThenInclude(ci => ci.Product)
                         .Where(c => c.Id == cartId);

            var cart = await query.FirstOrDefaultAsync();
            return cart;
        }

        public async Task<Cart> GetCartByUserIdAsync(Guid userId)
        {
            var query = _carts.AsQueryable();
            query = query.Include(c => c.CartItems)
                         .ThenInclude(ci => ci.Product);
            query = query.Where(c => c.UserId == userId);

            var cart = await query.FirstOrDefaultAsync();
            return cart;
        }

        public async Task<Cart> UpdateCartAsync(Cart updatedCart)
        {
            _context.Carts.Update(updatedCart);
            await _context.SaveChangesAsync();
            return updatedCart;
        }
    }
}
