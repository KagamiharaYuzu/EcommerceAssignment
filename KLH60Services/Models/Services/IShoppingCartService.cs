using System.Threading.Tasks;
using StoreClassLibrary;

namespace KLH60Services.Models.Services
{
    public interface IShoppingCartService
    {
        Task CreateShoppingCart(ShoppingCart shoppingCart);

        Task DeleteShoppingCart(int cartId);

        Task<ShoppingCart> GetCustomerShoppingCart(int custId);

        Task<ShoppingCart> GetShoppingCart(int id);

        Task UpdateShoppingCart(ShoppingCart shoppingCart);
    }
}