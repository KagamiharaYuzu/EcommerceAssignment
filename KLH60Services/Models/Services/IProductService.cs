using System.Collections.Generic;
using System.Threading.Tasks;
using StoreClassLibrary;

namespace KLH60Services.Models.Services
{
    public interface IProductService
    {
        Task CreateProduct(Product prod);

        Task<Product> GetProduct(int prodId);

        Task UpdateProduct(Product prod);

        Task DeleteProduct(int prodId);

        Task ReduceProductStock(int prodId, int amount = 1);

        Task RestoreProductStock(int prodId, int amount = 1);

        Task<IEnumerable<Product>> GetAllProducts();

        Task<IEnumerable<Product>> GetProductsByCategory(int catId);

        Task<IEnumerable<ProductsPerCategory>> GetAllProductsByCategory();

        Task<IEnumerable<Product>> Search(string searchTerm);
    }
}