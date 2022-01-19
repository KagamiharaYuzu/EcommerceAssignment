using StoreClassLibrary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KLH60Services.Models.Services
{
    public interface IOrderItemService
    {
        Task<OrderItem> GetOrderItem(int id);

        Task<IEnumerable<OrderItem>> GetOrderItems(int id);

        Task CreateOrderItem(OrderItem oItem);

        Task UpdateOrderItem(OrderItem oItem);

        Task DeleteOrderItem(int id);
    }
}