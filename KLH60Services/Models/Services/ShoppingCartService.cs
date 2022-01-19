using Microsoft.EntityFrameworkCore;
using StoreClassLibrary;
using System;
using System.Threading.Tasks;

namespace KLH60Services.Models.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        public readonly StoreServiceContext _db;

        public ShoppingCartService(StoreServiceContext db) => _db = db;

        public async Task<ShoppingCart> GetShoppingCart(int id) => await ShoppingCartExists(id) ? await _db.ShoppingCarts.AsNoTracking().FirstAsync(cart => cart.CartId == id) : throw new ArgumentException("Please select a valid shopping cart", nameof(id));

        public async Task<ShoppingCart> GetCustomerShoppingCart(int custId) => await _db.ShoppingCarts.AsNoTracking().AnyAsync(cart => cart.CartCustId == custId) ? await _db.ShoppingCarts.AsNoTracking().FirstOrDefaultAsync(cart => cart.CartCustId == custId) : throw new ArgumentException("No shopping carts found for the customer specified", nameof(custId));

        public async Task CreateShoppingCart(ShoppingCart shoppingCart)
        {
            if (shoppingCart is null)
                throw new ArgumentNullException(nameof(shoppingCart), "Invalid Shopping Cart specified");
            await _db.ShoppingCarts.AddAsync(shoppingCart);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateShoppingCart(ShoppingCart shoppingCart)
        {
            if (shoppingCart is null)
                throw new ArgumentNullException(nameof(shoppingCart), "Invalid Shopping Cart specified");
            _db.ShoppingCarts.Update(shoppingCart);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteShoppingCart(int cartId)
        {
            if (!await ShoppingCartExists(cartId))
                throw new ArgumentException("Please select a valid shopping cart to delete.", nameof(cartId));
            _db.ShoppingCarts.Remove(await _db.ShoppingCarts.FirstAsync(cart => cart.CartId == cartId));
            await _db.SaveChangesAsync();
        }

        private async Task<bool> ShoppingCartExists(int id) => await _db.ShoppingCarts.AsNoTracking().AnyAsync(e => e.CartId == id);
    }
}