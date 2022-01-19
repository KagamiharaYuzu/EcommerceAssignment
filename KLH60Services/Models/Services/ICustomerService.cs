using System.Collections.Generic;
using StoreClassLibrary;
using System.Threading.Tasks;

namespace KLH60Services.Models.Services
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomer(int id);

        Task<IEnumerable<Customer>> GetCustomers();

        Task<Customer> GetCustomerByEmail(string email);

        Task CreateCustomer(Customer cust);

        Task UpdateCustomer(Customer cust);

        Task RemoveCustomer(int id);
    }
}