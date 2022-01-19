using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreClassLibrary;

namespace KLH60Services.Models.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly StoreServiceContext _db;

        public OrderItemService(StoreServiceContext db) => _db = db;

        public async Task CreateOrderItem(OrderItem oItem)
        {
            if (oItem is null)
                throw new ArgumentNullException(nameof(oItem), "Invalid Order Item specified");
            await _db.OrderItems.AddAsync(oItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteOrderItem(int id)
        {
            if (!await OrderItemExists(id))
                throw new ArgumentException("No such order item exists in the database.", nameof(id));
            _db.OrderItems.Remove(await _db.OrderItems.FirstAsync(oItem => oItem.OrderItemId == id));
            await _db.SaveChangesAsync();
        }

        public async Task<OrderItem> GetOrderItem(int id) => !await OrderItemExists(id) ? throw new ArgumentException("Invalid Order Item Specified", nameof(id)) : await _db.OrderItems.AsNoTracking().FirstAsync(oItem => oItem.OrderItemId == id);

        public async Task<IEnumerable<OrderItem>> GetOrderItems(int ordId) => await _db.OrderItems.Where(oi => oi.OrderId == ordId).AsNoTracking().ToListAsync();

        public async Task UpdateOrderItem(OrderItem oItem)
        {
            if (oItem is null)
                throw new ArgumentNullException(nameof(oItem), "Invalid Order Item specified");
            _db.OrderItems.Update(oItem);
            await _db.SaveChangesAsync();
        }

        private async Task<bool> OrderItemExists(int id) => await _db.OrderItems.AsNoTracking().AnyAsync(e => e.OrderItemId == id);
    }
}