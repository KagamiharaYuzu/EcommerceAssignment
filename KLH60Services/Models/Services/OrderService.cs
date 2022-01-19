using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreClassLibrary;

namespace KLH60Services.Models.Services
{
    public class OrderService : IOrderService
    {
        private readonly StoreServiceContext _db;

        public OrderService(StoreServiceContext db) => _db = db;

        public async Task CreateOrder(Order ord)
        {
            if (ord is null)
                throw new ArgumentNullException(nameof(ord), "Invalid order received");
            await _db.Orders.AddAsync(ord);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetCustomerOrders(int custId) => (!await _db.Orders.AsNoTracking().AnyAsync(cust => cust.OrderCustId == custId)) ? throw new ArgumentException("This customer doesn't have any orders", nameof(custId)) : await _db.Orders.Where(ord => ord.OrderCustId == custId).AsNoTracking().ToListAsync();

        public async Task<Order> GetOrder(int id) => await OrderExist(id) ? await _db.Orders.FirstOrDefaultAsync(ord => ord.OrderId == id) : throw new ArgumentException("Order not found", nameof(id));

        public async Task<IEnumerable<Order>> GetOrders() => await _db.Orders.AsNoTracking().ToListAsync();

        public async Task<IEnumerable<Order>> GetOrdersByDate(DateTime date) => await _db.Orders.Where(ord => ord.DateCreated.Date == date || ord.DateFulfiled.Value.Date == date).AsNoTracking().ToListAsync();

        public async Task UpdateOrder(Order ord)
        {
            if (ord is null)
                throw new ArgumentNullException(nameof(ord), "Invalid order received");
            _db.Orders.Update(ord);
            await _db.SaveChangesAsync();
        }

        private async Task<bool> OrderExist(int id) => await _db.Orders.AsNoTracking().AnyAsync(ord => ord.OrderId == id);
    }
}