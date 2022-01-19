using System.Collections.Generic;
using StoreClassLibrary;
using System.Threading.Tasks;

namespace KLH60Services.Models.Services
{
    public interface ICartItemService
    {
        Task<CartItem> GetCartItem(int id);

        /// <summary>
        /// gets all the items in a cart with the specified cart id
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns>List of cart items belonging to the specified cart</returns>
        Task<IEnumerable<CartItemDisplay>> GetCartItems(int cartId);

        Task CreateCartItem(CartItem cItem);

        Task UpdateCartItem(CartItem cItem);

        Task DeleteCartItem(int id);

        /// <summary>
        /// Delete all the cart items belonging to a certain shopping cart
        /// </summary>
        /// <param name="id">id of the shopping cart to clear</param>
        /// <returns>void</returns>
        Task ClearItemsInCart(int id);
    }
}