using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreClassLibrary;

namespace KLH60Services.Models.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly StoreServiceContext _db;

        public CartItemService(StoreServiceContext db) => _db = db;

        public async Task CreateCartItem(CartItem cItem)
        {
            _ = CheckIfItemIsNull(cItem);
            if (await _db.CartItems.AnyAsync(item => item.ProductId == cItem.ProductId))
            {
                var cartItem = await _db.CartItems.FirstAsync(item => item.ProductId == cItem.ProductId);
                cartItem.Quantity++;
                await UpdateCartItem(cartItem);
            }
            else
            {
                await _db.CartItems.AddAsync(cItem);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteCartItem(int id)
        {
            if (await CartItemExists(id))
            {
                _db.CartItems.Remove(await _db.CartItems.FirstOrDefaultAsync(ci => ci.CartItemId == id));
                await _db.SaveChangesAsync();
            }
            else throw new ArgumentException("Cart Item to delete not found", nameof(id));
        }

        public async Task<CartItem> GetCartItem(int id)
        {
            if (id == 0)
                throw new ArgumentException("Cart Item Id not specified", nameof(id));
            else if (await CartItemExists(id))
                return await _db.CartItems.AsNoTracking().FirstOrDefaultAsync(cItem => cItem.CartItemId == id);
            else throw new Exception("Item not found");
        }

        public async Task ClearItemsInCart(int id) // here it's the cart id not the cart item id
        {
            if (id == 0)
                throw new ArgumentException("Cart Item Id not specified", nameof(id));
            await _db.CartItems.AsNoTracking().Where(cItem => cItem.CartId == id).ForEachAsync(citem => _db.CartItems.Remove(citem));
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<CartItemDisplay>> GetCartItems(int cartId)
        {
            if (cartId != 0)
                return await _db.CartItems.Where(cItem => cItem.CartId == cartId).Join(_db.Products, citem => citem.ProductId, prod => prod.ProductId, (citem, prod) => new CartItemDisplay(citem, prod.Description)).AsNoTracking().ToListAsync();
            else throw new ArgumentException("Invalid shopping cart specified");
        }

        public async Task UpdateCartItem(CartItem cItem)
        {
            _ = CheckIfItemIsNull(cItem);
            _db.CartItems.Update(cItem);
            await _db.SaveChangesAsync();
        }

        private static bool CheckIfItemIsNull(CartItem cartItem) => cartItem == null ? throw new ArgumentNullException(nameof(cartItem), "Please provide a valid cart item.") : true;

        private async Task<bool> CartItemExists(int id) => await _db.CartItems.AnyAsync(e => e.CartItemId == id);
    }
}