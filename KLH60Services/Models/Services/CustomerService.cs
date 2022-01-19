using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreClassLibrary;

namespace KLH60Services.Models.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly StoreServiceContext _db;

        public CustomerService(StoreServiceContext db) => _db = db;

        public async Task CreateCustomer(Customer cust)
        {
            if (cust is null)
                throw new ArgumentNullException(nameof(cust), "Invalid customer received");
            await _db.Customers.AddAsync(cust);
            await _db.SaveChangesAsync();
        }

        public async Task<Customer> GetCustomer(int id) => !await CustomerExists(id) ? throw new ArgumentException("Customer requested was not found.") : await _db.Customers.AsNoTracking().FirstOrDefaultAsync(cust => cust.CustomerId == id);

        public async Task<Customer> GetCustomerByEmail(string email) => !await _db.Customers.AsNoTracking().AnyAsync(e => e.Email.Equals(email)) ? throw new ArgumentException("Customer requested was not found.") : await _db.Customers.AsNoTracking().FirstOrDefaultAsync(cust => cust.Email.Equals(email));

        public async Task<IEnumerable<Customer>> GetCustomers() => await _db.Customers.AsNoTracking().ToListAsync();

        public async Task RemoveCustomer(int id)
        {
            if (!await CustomerExists(id))
                throw new ArgumentException("Customer to remove was not found", nameof(id));
            if (await _db.ShoppingCarts.AsNoTracking().AnyAsync(sc => sc.CartCustId == id) || await _db.Orders.AsNoTracking().AnyAsync(ord => ord.OrderCustId == id))
                throw new ArgumentException("customer cannot be deleted as they have an order or an ongoing shopping cart");
            Customer cust = await _db.Customers.FirstOrDefaultAsync(cust => cust.CustomerId == id);
            _db.Customers.Remove(cust);
            AspNetUser user = await _db.AspNetUsers.FirstOrDefaultAsync(us => us.Id == cust.AspnetUserId);
            if(user is not null)
                _db.AspNetUsers.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateCustomer(Customer cust)
        {
            if (cust is null)
                throw new ArgumentNullException(nameof(cust), "Invalid Customer received");
            _db.Customers.Update(cust);
            await _db.SaveChangesAsync();
        }

        private async Task<bool> CustomerExists(int id) => await _db.Customers.AsNoTracking().AnyAsync(e => e.CustomerId == id);
    }
}