namespace Ecommerce.Core.src.RepoAbstract
{
    public interface ICartItemRepo
    {
        Task<bool> DeleteCartItemByIdAsync(Guid cartItemId);
    }
}