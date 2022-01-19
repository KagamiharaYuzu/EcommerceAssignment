using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreClassLibrary;

namespace KLH60Services.Models.Services
{
    public interface IOrderService
    {
        Task<Order> GetOrder(int id);

        /// <summary>
        /// Get all the orders for a specified customer
        /// </summary>
        /// <param name="custId">The Customer ID</param>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetCustomerOrders(int custId);

        Task<IEnumerable<Order>> GetOrders();

        Task<IEnumerable<Order>> GetOrdersByDate(DateTime date);

        Task CreateOrder(Order ord);

        Task UpdateOrder(Order ord);
    }
}